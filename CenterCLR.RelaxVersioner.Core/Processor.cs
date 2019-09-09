/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016-2019 Kouji Matsui (@kozy_kekyo, @kekyo2)
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
using CenterCLR.RelaxVersioner.Writers;

using LibGit2Sharp;

namespace CenterCLR.RelaxVersioner
{
    public sealed class Processor
    {
        private static readonly Branch[] emtyBranches = new Branch[0];
        private static readonly Tag[] emptyTags = new Tag[0];

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

        private static Version LookupVersionLabel(
            Branch targetBranch,
            Dictionary<string, Tag[]> tags)
        {
            Debug.Assert(tags != null);

            if (targetBranch == null)
            {
                return Version.Default;
            }

            return targetBranch.Commits.
                Select((commit, index) => new { commit, index, tags = tags.GetValue(commit.Sha, emptyTags) }).
                Where(entry => entry.tags.Length >= 1).
                SelectMany(entry => entry.tags.
                    Select(tag => new { entry.index, version = Version.TryParse(tag.GetFriendlyName(), out var version) ? (Version?)version : null }).
                    Where(tagEntry => tagEntry.version.HasValue).
                    Select(tagEntry => Utilities.IncrementLastVersionComponent(tagEntry.version.Value, tagEntry.index))).
                DefaultIfEmpty(Version.Default).
                First();
        }

        private static Result WriteVersionSourceFile(
            Logger logger,
            WriterBase writer,
            string outputFilePath,
            Branch branchHint,
            Dictionary<string, Tag[]> tagsDictionary,
            Dictionary<string, Branch[]> branchesDictionary,
            string buildIdentifier,
            DateTimeOffset generated,
            IEnumerable<Rule> ruleSet,
            IEnumerable<string> importSet,
            bool isDryRun)
        {
            Debug.Assert(string.IsNullOrWhiteSpace(outputFilePath) == false);
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
                GetValue(commitId, emtyBranches).
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
                    {"buildIdentifier", buildIdentifier}
                };

            foreach (var entry in keyValues)
            {
                logger.Message(LogImportance.Low, "Values: {0}={1}", entry.Key, entry.Value);
            }

            if (!isDryRun)
            {
                writer.Write(outputFilePath, keyValues, generated, ruleSet, importSet);
            }

            return new Result(
                versionLabel,
                commitId,
                targetBranch.GetFriendlyName(),
                tags,
                committer.When,
                author.ToString(),
                committer.ToString(),
                commit.Message);
        }

        public Result Run(
            string projectDirectory,
            string outputFilePath,
            string language,
            string buildIdentifier,
            bool isDryRun)
        {
            var writer = writers[language];

            var elementSets = Utilities.GetElementSets(
                Utilities.LoadRuleSets(projectDirectory).
                    Concat(new[] { Utilities.GetDefaultRuleSet() }));

            var elementSet = elementSets[language];
            var importSet = Utilities.AggregateImports(elementSet);
            var ruleSet = Utilities.AggregateRules(elementSet);

            // Traverse git repository between projectDirectory and the root.
            var repository = Utilities.OpenRepository(logger, projectDirectory);

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
                    outputFilePath,
                    repository?.Head,
                    tags,
                    branches,
                    buildIdentifier,
                    DateTimeOffset.Now,
                    ruleSet,
                    importSet,
                    isDryRun);
            }
            finally
            {
                repository?.Dispose();
            }
        }
    }
}
