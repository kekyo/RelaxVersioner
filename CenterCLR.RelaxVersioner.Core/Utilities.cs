////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using LibGit2Sharp;

using RelaxVersioner.Writers;

namespace RelaxVersioner
{
    internal static class Utilities
    {
        private static readonly char[] directorySeparatorChar_ =
            { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        public static Dictionary<string, WriterBase> GetWriters()
        {
            return typeof(Utilities).Assembly.
                GetTypes().
                Where(type => type.IsSealed && type.IsClass && typeof(WriterBase).IsAssignableFrom(type)).
                Select(type => (WriterBase)Activator.CreateInstance(type)).
                ToDictionary(writer => writer.Language, StringComparer.InvariantCultureIgnoreCase);
        }

        private static T TraversePathToRoot<T>(string candidatePath, Func<string, T> action)
            where T : class
        {
            var path = Path.GetFullPath(candidatePath).
                TrimEnd(directorySeparatorChar_);

            while (true)
            {
                var result = action(path);
                if (result != null)
                {
                    return result;
                }

                var index = path.LastIndexOfAny(directorySeparatorChar_);
                if (index == -1)
                {
                    return null;
                }

                path = path.Substring(0, index);
            }
        }

        public static string GetDirectoryNameWithoutTrailingSeparator(string path) =>
            path.TrimEnd(directorySeparatorChar_);

        public static string GetDirectoryNameWithTrailingSeparator(string path) =>
            GetDirectoryNameWithoutTrailingSeparator(path) + Path.DirectorySeparatorChar;

        public static Repository OpenRepository(Logger logger, string candidatePath)
        {
            var repository = TraversePathToRoot(candidatePath, path =>
            {
                if (Directory.Exists(Path.Combine(path, ".git")))
                {
                    string GetNativeLibraryPath()
                    {
                        try
                        {
                            return GlobalSettings.NativeLibraryPath ?? "(null)";
                        }
                        catch
                        {
                            return "(unspecified)";
                        }
                    }
                    
                    logger.Message(LogImportance.Low, "libgit2sharp.NativeLibraryPath, Path={0}", GetNativeLibraryPath());

                    try
                    {
                        var r = new Repository(GetDirectoryNameWithTrailingSeparator(path));
                        logger.Message(LogImportance.Low, "Repository opened, Path={0}", path);
                        return r;
                    }
                    catch (RepositoryNotFoundException ex)
                    {
                        logger.Message(LogImportance.Low, ex, "Cannot open repository, Path={0}", path);
                    }
                }
                else
                {
                    logger.Message(LogImportance.Low, "This directory doesn't contain repository, Path={0}", path);
                }

                return null;
            });

            if (repository == null)
            {
                logger.Warning("Repository not found, CandidatePath={0}", candidatePath);
            }

            return repository;
        }

        public static TValue GetValue<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue)
        {
            Debug.Assert(dictionary != null);
            Debug.Assert(key != null);

            if (dictionary.TryGetValue(key, out TValue value) == false)
            {
                value = defaultValue;
            }

            return value;
        }

        public static Version GetSafeVersionFromDate(DateTimeOffset date)
        {
            // Second range: 0..43200 (2sec prec.)
            return new Version(
                date.Year,
                date.Month,
                date.Day,
                (int)(date.TimeOfDay.TotalSeconds / 2));
        }

        public static string GetIntDateVersionFromDate(DateTimeOffset date)
        {
            return date.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        public static string GetEpochIntDateVersionFromDate(DateTimeOffset date)
        {
            return date.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
        }

        public static IEnumerable<XElement> LoadRuleSets(string candidatePath)
        {
            Debug.Assert(candidatePath != null);

            var path = Path.GetFullPath(candidatePath).
                TrimEnd(directorySeparatorChar_);

            while (true)
            {
                var rulePath = Path.Combine(path, "RelaxVersioner.rules");
                if (File.Exists(rulePath))
                {
                    XElement element = null;
                    try
                    {
                        element = XElement.Load(rulePath);
                    }
                    catch
                    {
                    }

                    if (element != null)
                    {
                        yield return element;
                    }
                }

                var index = path.LastIndexOfAny(directorySeparatorChar_);
                if (index == -1)
                {
                    break;
                }

                path = path.Substring(0, index);
            }
        }

        public static Dictionary<string, XElement> GetElementSets(IEnumerable<XElement> ruleSets)
        {
            Debug.Assert(ruleSets != null);

            return
                (from ruleSet in ruleSets
                 where (ruleSet != null) && (ruleSet.Name.LocalName == "RelaxVersioner")
                 from rules in ruleSet.Elements("WriterRules")
                 from language in rules.Elements("Language")
                 where !string.IsNullOrWhiteSpace(language?.Value)
                 select new { language, rules }).
                GroupBy(
                    entry => entry.language.Value.Trim(),
                    entry => entry.rules,
                    StringComparer.InvariantCultureIgnoreCase).
                ToDictionary(
                    g => g.Key,
                    g => g.First(),
                    StringComparer.InvariantCultureIgnoreCase);
        }

        public static IEnumerable<string> AggregateImports(XElement wrules)
        {
            return (from import in wrules.Elements("Import")
                    select import.Value.Trim());
        }

        public static IEnumerable<Rule> AggregateRules(XElement wrules)
        {
            return (from rule in wrules.Elements("Rule")
                    let name = rule.Attribute("name")
                    let key = rule.Attribute("key")
                    where !string.IsNullOrWhiteSpace(name?.Value)
                    select new Rule(name.Value.Trim(), key?.Value.Trim(), rule.Value.Trim()));
        }

        public static XElement GetDefaultRuleSet()
        {
            var type = typeof(Utilities);
            using (var stream = type.Assembly.GetManifestResourceStream(
                type, "DefaultRuleSet.rules"))
            {
                return XElement.Load(stream);
            }
        }

        public static string GetFriendlyName<TObject>(this ReferenceWrapper<TObject> refer)
            where TObject : GitObject =>
            (refer.CanonicalName == "(no branch)") ?
                "HEAD" :
                string.IsNullOrWhiteSpace(refer.FriendlyName) ? refer.CanonicalName : refer.FriendlyName;

        public static Version IncrementLastVersionComponent(Version version, int value)
        {
            if (version.Revision.HasValue)
            {
                return new Version(
                    version.Major,
                    version.Minor.Value,
                    version.Build.Value,
                    version.Revision.Value + value);
            }
            else if (version.Build.HasValue)
            {
                return new Version(
                    version.Major,
                    version.Minor.Value,
                    version.Build.Value + value);
            }
            else if(version.Minor.HasValue)
            {
                return new Version(
                    version.Major,
                    version.Minor.Value + value);
            }
            else
            {
                return new Version(
                    version.Major + value);
            }
        }
    }
}
