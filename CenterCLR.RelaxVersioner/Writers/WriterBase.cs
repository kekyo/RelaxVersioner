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
using LibGit2Sharp;

namespace CenterCLR.RelaxVersioner.Writers
{
    internal abstract class WriterBase
    {
        private static readonly System.Version zeroVersion_ = new System.Version(0, 0, 0, 0);

        public abstract string Language { get; }

        public virtual string Write(
            string targetPath,
            Branch branch,
            Dictionary<string, IEnumerable<Tag>> tags,
            Dictionary<string, IEnumerable<Branch>> branches,
            bool requireMetadataAttribute,
            DateTimeOffset generated,
            ICollection<Rule> ruleSet)
        {
            Debug.Assert(string.IsNullOrWhiteSpace(targetPath) == false);
            Debug.Assert(tags != null);
            Debug.Assert(branches != null);
            Debug.Assert(ruleSet != null);

            var unknownBranch = new UnknownBranch(generated);

            var altBranch = branch ?? unknownBranch;
            var commit = altBranch.Commits.FirstOrDefault() ?? unknownBranch.Commits.First();

            var targetFolder = Path.GetDirectoryName(targetPath);
            if (Directory.Exists(targetFolder) == false)
            {
                try
                {
                    // Construct sub folders (ex: obj\Debug).
                    // May fail if parallel-building on MSBuild, ignoring exceptions.
                    Directory.CreateDirectory(targetFolder);
                }
                catch
                {
                }
            }

            using (var tw = File.CreateText(targetPath))
            {
                this.WriteComment(tw, "This is auto-generated version information attributes by CenterCLR.RelaxVersioner.{0}",
                    this.GetType().Assembly.GetName().Version);
                this.WriteComment(tw, "Do not edit.");
                this.WriteComment(tw, "Generated date: {0:R}", generated);
                tw.WriteLine();

                this.WriteBeforeBody(tw, requireMetadataAttribute);

                var namespaces = Utilities.AggregateNamespacesFromRuleSet(ruleSet);
                foreach (var namespaceName in namespaces)
                {
                    this.WriteImport(tw, namespaceName);
                }
                tw.WriteLine();

                var commitId = commit.Sha;
                var author = commit.Author;
                var committer = commit.Committer;

                var altBranches = string.Join(
                    ",",
                    branches.GetValue(commitId, Enumerable.Empty<Branch>()).
                        Select(b => b.Name));
                var altTags = string.Join(
                    ",",
                    tags.GetValue(commitId, Enumerable.Empty<Tag>()).
                        Select(b => b.Name));

                var safeVersion = Utilities.GetSafeVersionFromDate(committer.When);
                var gitLabel = Utilities.GetLabelWithFallback(altBranch, tags, branches) ?? zeroVersion_;

                var keyValues = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
                {
                    {"generated", generated},
                    {"branch", altBranch},
                    {"branches", altBranches},
                    {"tags", altTags},
                    {"commit", commit},
                    {"author", author},
                    {"committer", committer},
                    {"commitId", commitId},
                    {"gitLabel", gitLabel},
                    {"safeVersion", safeVersion}
                };

                foreach (var rule in ruleSet)
                {
                    var formattedValue = Named.Format(rule.Format, keyValues);
                    if (!string.IsNullOrWhiteSpace(rule.Key))
                    {
                        this.WriteAttributeWithArguments(tw, rule.Name, rule.Key, formattedValue);
                    }
                    else
                    {
                        this.WriteAttributeWithArguments(tw, rule.Name, formattedValue);
                    }
                }
                tw.WriteLine();

                this.WriteAfterBody(tw, requireMetadataAttribute);

                tw.Flush();

                return gitLabel.ToString();
            }
        }

        protected virtual void WriteComment(TextWriter tw, string format, params object[] args)
        {
            tw.WriteLine("// " + format, args);
        }

        protected virtual void WriteBeforeBody(TextWriter tw, bool requireMetadataAttribute)
        {
        }

        protected abstract void WriteAttribute(TextWriter tw, string name, string args);

        protected virtual string GetArgumentString(string argumentValue)
        {
            return string.Format("\"{0}\"", argumentValue.Replace("\"", "\"\""));
        }

        private void WriteAttributeWithArguments(TextWriter tw, string name, params object[] args)
        {
            this.WriteAttribute(
                tw,
                name,
                string.Join(",", args.Select(arg => this.GetArgumentString((arg != null) ? arg.ToString() : string.Empty))));
        }

        protected virtual void WriteImport(TextWriter tw, string namespaceName)
        {
        }

        protected virtual void WriteAfterBody(TextWriter tw, bool requireMetadataAttribute)
        {
        }
    }
}
