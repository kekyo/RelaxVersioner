/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
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
using System.Diagnostics;
using LibGit2Sharp;

namespace CenterCLR.RelaxVersioner.Writers
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
