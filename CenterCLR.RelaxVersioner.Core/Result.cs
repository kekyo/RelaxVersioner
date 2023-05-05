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

namespace RelaxVersioner;

public readonly struct Result
{
    public readonly Version Version;
    public readonly string ShortVersion;
    public readonly Version SafeVersion;
    public readonly string IntDateVersion;
    public readonly string EpochIntDateVersion;
    public readonly string CommitId;
    public readonly string[] Tags;
    public readonly string Branch;
    public readonly DateTimeOffset Date;
    public readonly string Author;
    public readonly string Committer;
    public readonly string Subject;
    public readonly string Body;

    public Result(Version version, string shortVersion, Version safeVersion,
        string intDateVersion, string epochIntDateVersion,
        string commitId,
        string branch, string[] tags, DateTimeOffset date,
        string author, string committer,
        string subject, string body)
    {
        this.Version = version;
        this.ShortVersion = shortVersion;
        this.SafeVersion = safeVersion;
        this.IntDateVersion = intDateVersion;
        this.EpochIntDateVersion = epochIntDateVersion;
        this.CommitId = commitId;
        this.Branch = branch;
        this.Tags = tags;
        this.Date = date;
        this.Author = author;
        this.Committer = committer;
        this.Subject = subject;
        this.Body = body;
    }
}
