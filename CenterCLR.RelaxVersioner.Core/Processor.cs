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
using System.Linq;
using CenterCLR.RelaxVersioner.Writers;

using LibGit2Sharp;

namespace CenterCLR.RelaxVersioner
{
    public sealed class Processor
    {
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

        public Result Run(string projectDirectory, string outputFilePath, string language, bool isDryRun)
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
                        g => g.ToList().AsEnumerable(),
                        StringComparer.InvariantCultureIgnoreCase) ??
                    new Dictionary<string, IEnumerable<Tag>>();

                var branches = (repository != null) ?
                    (from branch in repository.Branches
                     where !branch.IsRemote
                     from commit in branch.Commits
                     group branch by commit.Sha).
                    ToDictionary(
                        g => g.Key,
                        g => g.ToList().AsEnumerable(),
                        StringComparer.InvariantCultureIgnoreCase) :
                    new Dictionary<string, IEnumerable<Branch>>();

                return writer.Write(
                    logger,
                    outputFilePath,
                    repository?.Head,
                    tags,
                    branches,
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
