/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016 Kouji Matsui (@kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CenterCLR.RelaxVersioner.Writers;
using LibGit2Sharp;

namespace CenterCLR.RelaxVersioner
{
    internal static class Utilities
    {
        private static readonly char[] versionSeparators_ = {'/', '-', '_'};

        public static Dictionary<string, WriterBase> GetWriters()
        {
            return typeof (Program).Assembly.
                GetTypes().
                Where(type => type.IsSealed && type.IsClass && typeof (WriterBase).IsAssignableFrom(type)).
                Select(type => (WriterBase) Activator.CreateInstance(type)).
                ToDictionary(writer => writer.Language, StringComparer.InvariantCultureIgnoreCase);
        }

        public static Repository OpenRepository(string candidatePath)
        {
            var currentPath = Path.GetFullPath(candidatePath).
                Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            while (true)
            {
                try
                {
                    return new Repository(currentPath + Path.DirectorySeparatorChar);
                }
                catch (RepositoryNotFoundException)
                {
                    var index = currentPath.LastIndexOfAny(
                        new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar});
                    if (index == -1)
                    {
                        return null;
                    }

                    currentPath = currentPath.Substring(0, index);
                }
            }
        }

        public static double GetFrameworkVersionNumber(string stringValue)
        {
            var split = stringValue.TrimStart('v').
                Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
            double value;
            double.TryParse(
                $"{(split.ElementAtOrDefault(0) ?? "0")}.{(split.ElementAtOrDefault(1) ?? "0")}",
                out value);
            return value;
        }

        private static int GetVersionNumber(string stringValue)
        {
            Debug.Assert(stringValue != null);

            ushort value;
            return ushort.TryParse(stringValue, out value) ? value : -1;
        }

        private static System.Version TryParseVersion(string versionString)
        {
            Debug.Assert(versionString != null);

            System.Version version;
            System.Version.TryParse(versionString, out version);
            return version;
        }

        public static System.Version GetVersionFromGitLabel(string label)
        {
            Debug.Assert(label != null);

            return label.
                Split(versionSeparators_, StringSplitOptions.RemoveEmptyEntries).
                Select(TryParseVersion).
                LastOrDefault(version => version != null); // Separate and search last valid version string
        }

        public static TValue GetValue<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue)
        {
            Debug.Assert(dictionary != null);
            Debug.Assert(key != null);

            TValue value;
            if (dictionary.TryGetValue(key, out value) == false)
            {
                value = defaultValue;
            }

            return value;
        }

        public static System.Version GetSafeVersionFromDate(DateTimeOffset date)
        {
            // Second range: 0..43200 (2sec prec.)
            return new System.Version(
                date.Year,
                date.Month,
                date.Day,
                (int)(date.TimeOfDay.TotalSeconds / 2));
        }

        public static XElement LoadRuleSet(string path)
        {
            Debug.Assert(path != null);

            try
            {
                var rulePath = Path.Combine(path, "RelaxVersioner.rules");
                if (File.Exists(rulePath) == false)
                {
                    return null;
                }

                return XElement.Load(rulePath);
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<string, XElement> GetElementSets(params XElement[] ruleSets)
        {
            Debug.Assert(ruleSets != null);

            return
                (from ruleSet in ruleSets
                 where (ruleSet != null) && (ruleSet.Name.LocalName == "RelaxVersioner")
                 from rules in ruleSet.Elements("WriterRules")
                 from language in rules.Elements("Language")
                 where !string.IsNullOrWhiteSpace(language?.Value)
                 select new {language, rules}).
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

        public static IEnumerable<string> AggregateNamespacesFromRuleSet(
            IEnumerable<Rule> ruleSet)
        {
            Debug.Assert(ruleSet != null);

            return
                (from rule in ruleSet
                 let symbolElements = rule.Name.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries)
                 select string.Join(".", symbolElements.Take(symbolElements.Length - 1))).
                Distinct();
        }

        public static XElement GetDefaultRuleSet()
        {
            var type = typeof (Utilities);
            using (var stream = type.Assembly.GetManifestResourceStream(
                type, "DefaultRuleSet.rules"))
            {
                return XElement.Load(stream);
            }
        }

        public static System.Version GetLabelWithFallback(
            Branch targetBranch,
            Dictionary<string, IEnumerable<Tag>> tags,
            Dictionary<string, IEnumerable<Branch>> branches)
        {
            Debug.Assert(tags != null);
            Debug.Assert(branches != null);

            if (targetBranch == null)
            {
                return null;
            }

            // First commit: Tags only
            // Second...   : Branches only
            //   If success label parse (GetVersionFromGitLabel), candidate it.
            var versions =
                (from commit in targetBranch.Commits
                 from label in
                    tags.GetValue(commit.Sha, Enumerable.Empty<Tag>()).Select(tag => GetVersionFromGitLabel(tag.FriendlyName))
                 select label).
                Concat(
                    from commit in targetBranch.Commits.Skip(1)
                    from label in
                        branches.GetValue(commit.Sha, Enumerable.Empty<Branch>()).Select(branch => GetVersionFromGitLabel(branch.FriendlyName))
                    select label);

            // Use first version, if no candidate then try current branch name.
            return versions.FirstOrDefault(label => label != null) ??
                GetVersionFromGitLabel(targetBranch.FriendlyName);
        }
    }
}
