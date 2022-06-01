////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

#nullable enable

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
