////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using LibGit2Sharp;

namespace RelaxVersioner.Writers
{
    internal sealed class UnknownCommit : Commit
    {
        private readonly Signature author_;
        private readonly Signature committer_;

        public UnknownCommit(DateTimeOffset when)
        {
            author_ = new Signature("(Unknown author)", "unknown@example.com", when);
            committer_ = new Signature("(Unknown committer)", "unknown@example.com", when);
        }

        public override string Sha =>
            "(Unknown commit id)";

        public override string Message =>
            "(Unknown commit message)";

        public override string MessageShort =>
            "(Unknown commit message)";

        public override Signature Author =>
            author_;

        public override Signature Committer =>
            committer_;

        public override string ToString() =>
            this.Sha;
    }
}
