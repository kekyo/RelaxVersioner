/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016-2018 Kouji Matsui (@kozy_kekyo, @kekyo2)
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
using System.Runtime.InteropServices;
using LibGit2Sharp;

namespace CenterCLR.RelaxVersioner
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var solutionDirectory = args[0];
                var projectDirectory = args[1];
                var targetPath = args[2];
                var targetFrameworkVersion = Utilities.GetFrameworkVersionNumber(args[3]);
                var targetFrameworkProfile = args[4];
                var language = args[5];

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

                    var gitLabel = writer.Write(
                        targetPath,
                        repository?.Head,
                        tags,
                        branches,
                        DateTimeOffset.Now,
                        ruleSet);

                    Console.WriteLine("RelaxVersioner: Generated versions code: Language={0}, Version={1}", language, gitLabel);
                }
                finally
                {
                    repository?.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("RelaxVersioner: " + ex.Message);
                return Marshal.GetHRForException(ex);
            }

            return 0;
        }
    }
}
