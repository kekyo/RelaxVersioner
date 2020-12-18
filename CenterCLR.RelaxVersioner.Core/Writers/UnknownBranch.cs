/////////////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
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
using System.Collections;
using System.Collections.Generic;
using LibGit2Sharp;

namespace RelaxVersioner.Writers
{
    internal sealed class UnknownBranch : Branch
    {
        private readonly ICommitLog commits_;

        public UnknownBranch(DateTimeOffset when) =>
            commits_ = new UnknownCommitLog(when);

        public override string FriendlyName =>
            "(Unknown branch)";

        public override string CanonicalName =>
            "(Unknown branch)";

        public override ICommitLog Commits =>
            commits_;

        public override string ToString() =>
            this.FriendlyName;

        private sealed class UnknownCommitLog : ICommitLog
        {
            private readonly IEnumerable<Commit> commits_;

            public UnknownCommitLog(DateTimeOffset when) =>
                commits_ = new[] {new UnknownCommit(when)};

            public CommitSortStrategies SortedBy =>
                CommitSortStrategies.Time;

            public IEnumerator<Commit> GetEnumerator() =>
                commits_.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() =>
                commits_.GetEnumerator();

            public override string ToString() =>
                "(Unknown commit log)";
        }
    }
}
