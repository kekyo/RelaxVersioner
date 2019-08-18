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
using Microsoft.Build.Framework;
using LibGit2Sharp;

namespace CenterCLR.RelaxVersioner
{
    public sealed class RelaxVersionerTask : Microsoft.Build.Utilities.Task
    {
        public RelaxVersionerTask()
        {
        }

        [Required]
        public ITaskItem SolutionDirectory
        {
            get; set;
        }

        [Required]
        public ITaskItem ProjectDirectory
        {
            get; set;
        }

        [Required]
        public ITaskItem OutputPath
        {
            get; set;
        }

        [Required]
        public string Language
        {
            get; set;
        }

        [Output]
        public string DetectedIdentity
        {
            get; set;
        }

        [Output]
        public string DetectedShortIdentity
        {
            get; set;
        }

        [Output]
        public string DetectedMessage
        {
            get; set;
        }

        public override bool Execute()
        {
            try
            {
                var writers = Utilities.GetWriters();
                var writer = writers[this.Language];

                var elementSets = Utilities.GetElementSets(
                    Utilities.LoadRuleSet(this.ProjectDirectory.ItemSpec),
                    Utilities.LoadRuleSet(this.SolutionDirectory.ItemSpec),
                    Utilities.GetDefaultRuleSet());

                var elementSet = elementSets[this.Language];
                var importSet = Utilities.AggregateImports(elementSet);
                var ruleSet = Utilities.AggregateRules(elementSet);

                // Traverse git repository between projectDirectory and the root.
                // Why use projectDirectory instead solutionDirectory ?
                //   Because solution file (*.sln) is only aggregate project's pointers.
                //   Some use case, solution file placed exterior of git work folder,
                //   but project folder always placed interior git work folder.
                var repository = Utilities.OpenRepository(this.ProjectDirectory.ItemSpec);

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

                    var result = writer.Write(
                        this.OutputPath.ItemSpec,
                        repository?.Head,
                        tags,
                        branches,
                        DateTimeOffset.Now,
                        ruleSet,
                        importSet);

                    this.DetectedIdentity = result.Identity;
                    this.DetectedShortIdentity = result.ShortIdentity;
                    this.DetectedMessage = result.Message;

                    base.Log.LogMessage(
                        $"RelaxVersioner: Generated versions code: Language={this.Language}, Version={this.DetectedIdentity}");
                }
                finally
                {
                    repository?.Dispose();
                }
            }
            catch (Exception ex)
            {
                base.Log.LogError($"RelaxVersioner: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}
