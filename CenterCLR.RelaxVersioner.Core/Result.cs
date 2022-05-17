////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

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
