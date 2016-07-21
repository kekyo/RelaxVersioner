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
using System.Linq;
using LibGit2Sharp;

namespace CenterCLR.RelaxVersioner
{
    internal static class Executor
    {
        public static int Execute(
            string solutionDirectory,
            string projectDirectory,
            string targetPath,
            double targetFrameworkVersion,
            string targetFrameworkProfile,
            string language,
            List<string> combineDefinitions,
            Action<string> outputWriter)
        {
            Debug.Assert(combineDefinitions != null);
            Debug.Assert(outputWriter != null);

            var writers = Utilities.GetWriters();
            var writer = writers[language];

            var ruleSets = Utilities.AggregateRuleSets(
                Utilities.LoadRuleSet(projectDirectory),
                Utilities.LoadRuleSet(solutionDirectory),
                Utilities.GetDefaultRuleSet());

            var ruleSet = ruleSets[language].ToList();

            // Traverse git repository between projectDirectory and the root.
            // Why use projectDirectory instead solutionDirectory ?
            //   Because solution file (*.sln) is only aggregate project's pointers.
            //   Some use case, solution file placed exterior of git work folder,
            //   but project folder always placed interior git work folder.
            var repository = Utilities.OpenRepository(projectDirectory);

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

                var targetBranch = repository?.Head;
                var generated = DateTimeOffset.Now;

                System.Version gitLabel;
                var keyValues = Utilities.ConstructFormatParameters(
                    targetBranch,
                    tags,
                    branches,
                    generated,
                    out gitLabel);

                writer.Write(
                    targetPath,
                    keyValues,
                    targetBranch,
                    (targetFrameworkVersion < 4.5) || !string.IsNullOrWhiteSpace(targetFrameworkProfile),
                    generated,
                    ruleSet,
                    combineDefinitions);

                outputWriter(string.Format(
                    "RelaxVersioner: Generated versions code: Language={0}, Version={1}", language, gitLabel));
            }
            finally
            {
                repository?.Dispose();
            }

            return 0;
        }
    }
}
