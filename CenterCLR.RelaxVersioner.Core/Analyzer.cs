////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version information inserter.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

#nullable enable

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using GitReader.Structures;

namespace RelaxVersioner;

internal static class Analyzer
{
    private readonly struct ScheduledCommit
    {
        public readonly Commit Commit;
        public readonly Commit[] Parents;

        public ScheduledCommit(
            Commit commit, Commit[] parents)
        {
            this.Commit = commit;
            this.Parents = parents;
        }

        public void Deconstruct(
            out Commit commit, out Commit[] parents)
        {
            commit = this.Commit;
            parents = this.Parents;
        }
    }

    private static Version IncrementLastVersionComponent(
        Version version)
    {
        if (version.Revision.HasValue)
        {
            return new(
                version.Major,
                version.Minor!.Value,
                version.Build!.Value,
                version.Revision.Value + 1);
        }
        else if (version.Build.HasValue)
        {
            return new(
                version.Major,
                version.Minor!.Value,
                version.Build.Value + 1);
        }
        else if (version.Minor.HasValue)
        {
            return new(
                version.Major,
                version.Minor.Value + 1);
        }
        else
        {
            return new(
                version.Major + 1);
        }
    }

    private static async Task<Version> LookupVersionLabelAsync(
        Commit commit, Dictionary<Commit, Version> reached, CancellationToken ct)
    {
        var scheduledStack = new Stack<ScheduledCommit>();
        var version = Version.Default;

        // Trace back to the parent commit repeatedly with the following conditions:
        // * If the commit has already been reached, get its version.
        // * If there is a recognizable version string in the tag, get its version.
        // * If the parent commit does not exist, get the default version.
        // * If other than the above, push the commit on the stack for later processing in reverse order.
        while (true)
        {
            if (reached.TryGetValue(commit, out var v1))
            {
                version = v1;
                break;
            }

            var found = false;
            foreach (var tag in commit.Tags)
            {
                if (Version.TryParse(tag.Name, out var v2))
                {
                    version = v2;
                    reached.Add(commit, version);
                    found = true;
                    break;
                }
            }
            if (found)
            {
                break;
            }

            var parents = await commit.GetParentCommitsAsync(ct);
            if (parents.Length == 0)
            {
                reached.Add(commit, version);
                break;
            }

            scheduledStack.Push(new(commit, parents));
            commit = parents[0];
        }

        // As long as there are commits stacked on the stack,
        // retrieve a commit from the stack, and if there is more than one parent commit for that commit:
        // * Recursively get versions from parent commits other than the primary one.
        // * Compare the versions obtained and store the largest version.
        // * Increment the version and make it the version of the current commit.
        while (scheduledStack.Count >= 1)
        {
            var (c, pcs) = scheduledStack.Pop();
            if (pcs.Length >= 2)
            {
                for (var index = 1; index < pcs.Length; index++)
                {
                    var v = await LookupVersionLabelAsync(pcs[index], reached, ct);
                    if (v.CompareTo(version) > 0)
                    {
                        version = v;
                    }
                }
            }

            version = IncrementLastVersionComponent(version);
            reached.Add(c, version);
        }

        return version;
    }

    public static Task<Version> LookupVersionLabelAsync(
        Commit commit, CancellationToken ct) =>
        LookupVersionLabelAsync(commit, new(), ct);

    public static async Task<Version> LookupVersionLabelAsync(
        Branch branch, CancellationToken ct)
    {
        // Get the head commit for the branch.
        var headCommit = await branch.GetHeadCommitAsync(ct);

        return await LookupVersionLabelAsync(headCommit, new(), ct);
    }
}
