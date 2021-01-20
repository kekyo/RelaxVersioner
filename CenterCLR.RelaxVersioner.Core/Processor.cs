/////////////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016-2021 Kouji Matsui (@kozy_kekyo, @kekyo2)
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
using System.Linq;
using RelaxVersioner.Writers;

using LibGit2Sharp;

namespace RelaxVersioner
{
    public sealed class ProcessorContext
    {
        public string ProjectDirectory;
        public string OutputPath;
        public string Language;
        public string Namespace;
        public string TargetFramework;
        public string TargetFrameworkIdentity;
        public string TargetFrameworkVersion;
        public string TargetFrameworkProfile;
        public bool GenerateStatic;
        public string BuildIdentifier;
    }

    public sealed class Processor
    {
        private static readonly Branch[] emptyBranches = Array.Empty<Branch>();
        private static readonly Tag[] emptyTags = Array.Empty<Tag>();

        private readonly Logger logger;
        private readonly Dictionary<string, WriterBase> writers;

        public Processor(Logger logger)
        {
            this.logger = logger;
            this.writers = Utilities.GetWriters();
            this.Languages = this.writers.Values.
                Select(writer => writer.Language).
                ToArray();
        }

        public string[] Languages { get; }

        private readonly struct TargetCommit
        {
            public readonly int StartDepth;
            public readonly Commit Commit;

            public TargetCommit(int startDepth, Commit commit)
            {
                this.StartDepth = startDepth;
                this.Commit = commit;
            }

            public override string ToString() =>
                $"StartDepth={this.StartDepth}, {this.Commit}";
        }

        private static Version LookupVersionLabel(
            Branch targetBranch,
            Dictionary<string, Tag[]> tagsDictionary)
        {
            Debug.Assert(tagsDictionary != null);

            var topCommit = targetBranch?.Commits?.FirstOrDefault();
            if (topCommit == null)
            {
                return Version.Default;
            }

            var reached = new HashSet<string>();
            var scheduled = new Stack<TargetCommit>();
            scheduled.Push(new TargetCommit(0, topCommit));

            var mainDepth = 0;

            while (scheduled.Count >= 1)
            {
                // Extract an analysis commit.
                var entry = scheduled.Pop();
                
                var currentCommit = entry.Commit;
                var currentDepth = entry.StartDepth;

                while (true)
                {
                    // Rejoined parent branch.
                    if (!reached.Add(currentCommit.Sha))
                    {
                        break;
                    }

                    // If found be applied tags at this commit:
                    if (tagsDictionary.TryGetValue(currentCommit.Sha, out var tags))
                    {
                        var filteredTags = tags.
                            Select(tag => Version.TryParse(tag.GetFriendlyName(), out var version) ? (Version?)version : null).
                            Where(version => version.HasValue).
                            Select(version => Utilities.IncrementLastVersionComponent(version.Value, currentDepth)).
                            ToArray();

                        // Found first valid tag.
                        if (filteredTags.Length >= 1)
                        {
                            return filteredTags[0];
                        }
                    }

                    // Found parents.
                    if ((currentCommit.Parents?.ToArray() is { Length: >= 1 } parents))
                    {
                        // Dive parent commit.
                        currentDepth++;

                        // Next commit is a primary parent.
                        currentCommit = parents[0];

                        // Enqueue analysis scheduling if it has multiple parents.
                        foreach (var parentCommit in parents.Skip(1))
                        {
                            scheduled.Push(new TargetCommit(currentDepth, parentCommit));
                        }
                    }
                    // Bottom of branch.
                    else
                    {
                        // Save depth if it's on boarding the main branch.
                        if (mainDepth == 0)
                        {
                            mainDepth = currentDepth;
                        }
                        
                        // Goes to next scheduled commit.
                        break;
                    }
                }
            }

            // Finally got the main branch depth.
            return Utilities.IncrementLastVersionComponent(Version.Default, mainDepth);
        }

        private static Result WriteVersionSourceFile(
            Logger logger,
            WriterBase writer,
            ProcessorContext context,
            Branch branchHint,
            Dictionary<string, Tag[]> tagsDictionary,
            Dictionary<string, Branch[]> branchesDictionary,
            DateTimeOffset generated,
            IEnumerable<Rule> ruleSet,
            IEnumerable<string> importSet)
        {
            Debug.Assert(tagsDictionary != null);
            Debug.Assert(branchesDictionary != null);
            Debug.Assert(ruleSet != null);
            Debug.Assert(importSet != null);

            var unknownBranch = new UnknownBranch(generated);

            var targetBranch = branchHint ?? unknownBranch;
            var commit = targetBranch.Commits.FirstOrDefault() ?? unknownBranch.Commits.First();

            var commitId = commit.Sha;
            var author = commit.Author;
            var committer = commit.Committer;

            var branches = branchesDictionary.
                GetValue(commitId, emptyBranches).
                Select(b => b.GetFriendlyName()).
                ToArray();
            var branchesString = string.Join(",", branches);

            var tags = tagsDictionary.
                GetValue(commitId, emptyTags).
                Select(b => b.GetFriendlyName()).
                ToArray();
            var tagsString = string.Join(",", tags);

            var safeVersion = Utilities.GetSafeVersionFromDate(committer.When);
            var versionLabel = LookupVersionLabel(targetBranch, tagsDictionary);

            var keyValues = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
                {
                    {"generated", generated},
                    {"branch", targetBranch},
                    {"branches", branchesString},
                    {"tags", tagsString},
                    {"commit", commit},
                    {"author", author},
                    {"committer", committer},
                    {"commitId", commitId},
                    {"versionLabel", versionLabel},
                    {"safeVersion", safeVersion},
                    {"buildIdentifier", context.BuildIdentifier},
                    {"namespace", context.Namespace},
                    {"tfm", context.TargetFramework},
                    {"tfid", context.TargetFrameworkIdentity},
                    {"tfv", context.TargetFrameworkVersion},
                    {"tfp", context.TargetFrameworkProfile},
                };

            foreach (var entry in keyValues)
            {
                logger.Message(LogImportance.Low, "Values: {0}={1}", entry.Key, entry.Value);
            }

            if (!string.IsNullOrWhiteSpace(context.OutputPath))
            {
                writer.Write(context, keyValues, generated, ruleSet, importSet);
            }

            return new Result(
                versionLabel,
                safeVersion,
                commitId,
                targetBranch.GetFriendlyName(),
                tags,
                committer.When,
                author.ToString(),
                committer.ToString(),
                commit.Message);
        }

        public Result Run(ProcessorContext context)
        {
            var writer = writers[context.Language];

            var elementSets = Utilities.GetElementSets(
                Utilities.LoadRuleSets(context.ProjectDirectory).
                    Concat(new[] { Utilities.GetDefaultRuleSet() }));

            var elementSet = elementSets[context.Language];
            var importSet = Utilities.AggregateImports(elementSet);
            var ruleSet = Utilities.AggregateRules(elementSet);

            // Traverse git repository between projectDirectory and the root.
            var repository = Utilities.OpenRepository(logger, context.ProjectDirectory);

            try
            {
                var tags = repository?.Tags.
                    Where(tag => tag.Target is Commit).
                    GroupBy(tag => tag.Target.Sha).
                    ToDictionary(
                        g => g.Key,
                        g => g.ToArray(),
                        StringComparer.InvariantCultureIgnoreCase) ??
                    new Dictionary<string, Tag[]>();

                var branches = (repository != null) ?
                    (from branch in repository.Branches
                     where !branch.IsRemote
                     from commit in branch.Commits
                     group branch by commit.Sha).
                    ToDictionary(
                        g => g.Key,
                        g => g.ToArray(),
                        StringComparer.InvariantCultureIgnoreCase) :
                    new Dictionary<string, Branch[]>();

                return WriteVersionSourceFile(
                    logger,
                    writer,
                    context,
                    repository?.Head,
                    tags,
                    branches,
                    DateTimeOffset.Now,
                    ruleSet,
                    importSet);
            }
            finally
            {
                repository?.Dispose();
            }
        }
    }
}
