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

namespace RelaxVersioner
{
    public struct Result
    {
        public readonly Version Version;
        public readonly string ShortVersion;
        public readonly Version SafeVersion;
        public readonly string CommitId;
        public readonly string[] Tags;
        public readonly string Branch;
        public readonly DateTimeOffset Date;
        public readonly string Author;
        public readonly string Committer;
        public readonly string Message;

        public Result(Version version, Version safeVersion, string commitId,
            string branch, string[] tags, DateTimeOffset date,
            string author, string committer,
            string message)
        {
            this.Version = version;
            this.ShortVersion = version.ToString(3);
            this.SafeVersion = safeVersion;
            this.CommitId = commitId;
            this.Branch = branch;
            this.Tags = tags;
            this.Date = date;
            this.Author = author;
            this.Committer = committer;
            this.Message = message;
        }
    }
}
