////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
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
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GitReader;
using GitReader.Structures;

using RelaxVersioner.Writers;

namespace RelaxVersioner;

internal static class Utilities
{
    private static readonly char[] directorySeparatorChar_ =
        { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        
    public static readonly Encoding UTF8 = new UTF8Encoding(false);

    [DebuggerStepThrough]
    public static async Task<(T0 v0, T1 v1)> Join<T0, T1>(
        Task<T0> task0, Task<T1> task1) =>
        new(await task0, await task1);

    public static string GetDirectoryPath(string path) =>
        Path.GetDirectoryName(path) switch
        {
            // Not accurate in Windows, but a compromise...
            null => Path.DirectorySeparatorChar.ToString(),
            "" => string.Empty,
            var dp => dp,
        };

    public static Dictionary<string, WriteProviderBase> GetWriteProviders()
    {
        return typeof(Utilities).Assembly.
            GetTypes().
            Where(type => type.IsSealed && type.IsClass && typeof(WriteProviderBase).IsAssignableFrom(type)).
            Select(type => (WriteProviderBase)Activator.CreateInstance(type)).
            ToDictionary(writer => writer.Language, StringComparer.InvariantCultureIgnoreCase);
    }

    private static async Task<T> TraversePathToRootAsync<T>(
        string candidatePath, Func<string, Task<T>> action)
        where T : class
    {
        var path = Path.GetFullPath(candidatePath).
            TrimEnd(directorySeparatorChar_);

        while (true)
        {
            var result = await action(path);
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

    public static async Task<StructuredRepository> OpenRepositoryAsync(
        Logger logger, string candidatePath)
    {
        var repository = await TraversePathToRootAsync(candidatePath, async path =>
        {
            var gitPath = Path.Combine(path, ".git");
            if (Directory.Exists(gitPath) ||
                File.Exists(gitPath))  // submodule
            {
                var r = await Repository.Factory.OpenStructureAsync(path);
                logger.Message(LogImportance.Low, "Repository opened, Path={0}", path);
                return r;
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
            (ushort)date.Year,
            (ushort)date.Month,
            (ushort)date.Day,
            (ushort)(date.TimeOfDay.TotalSeconds / 2));
    }

    public static string GetIntDateVersionFromDate(DateTimeOffset date)
    {
        return date.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
    }

    public static string GetEpochIntDateVersionFromDate(DateTimeOffset date)
    {
        // Second range: 0..43200 (2sec prec.)
        return ((ushort)(date.TimeOfDay.TotalSeconds / 2)).
            ToString(CultureInfo.InvariantCulture);
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

    public static string MakeSaferChars(string str, string replacement)
    {
        var sb = new StringBuilder(str.Length);
        foreach (var ch in str)
        {
            if (char.IsControl(ch) && char.IsPunctuation(ch) &&
                char.IsSeparator(ch) && char.IsSymbol(ch) &&
                char.IsWhiteSpace(ch))
            {
                sb.Append(replacement);
            }
            else
            {
                sb.Append(ch);
            }
        }
        return sb.ToString();
    }

    public static string NormalizeControlCharsForCLike(string str)
    {
        var sb = new StringBuilder(str.Length + 20);
        foreach (var ch in str)
        {
            if (char.IsControl(ch))
            {
                sb.Append("\\u");
                sb.Append(((int)ch).ToString("x4", CultureInfo.InvariantCulture));
            }
            else if (ch == '\\')
            {
                sb.Append("\\\\");
            }
            else if (ch == '"')
            {
                sb.Append("\\\"");
            }
            else
            {
                sb.Append(ch);
            }
        }
        return sb.ToString();
    }
}
