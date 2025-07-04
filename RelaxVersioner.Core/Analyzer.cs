﻿////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GitReader.IO;
using GitReader.Primitive;

namespace RelaxVersioner;

internal static class Analyzer
{
    private readonly struct ScheduledCommit
    {
        public readonly PrimitiveCommit Commit;
        public readonly PrimitiveCommit[] Parents;

        public ScheduledCommit(
            PrimitiveCommit commit, PrimitiveCommit[] parents)
        {
            this.Commit = commit;
            this.Parents = parents;
        }

        public void Deconstruct(
            out PrimitiveCommit commit, out PrimitiveCommit[] parents)
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
                (ushort)(version.Revision.Value + 1));
        }
        else if (version.Build.HasValue)
        {
            return new(
                version.Major,
                version.Minor!.Value,
                (ushort)(version.Build.Value + 1));
        }
        else if (version.Minor.HasValue)
        {
            return new(
                version.Major,
                (ushort)(version.Minor.Value + 1));
        }
        else
        {
            return new(
                (ushort)(version.Major + 1));
        }
    }

    private static async Task<Version> LookupVersionLabelRecursiveAsync(
        PrimitiveRepository repository,
        PrimitiveCommit commit,
        Dictionary<PrimitiveCommit, Version> reached, CancellationToken ct)
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

            // Detected mostly larger version tag.
            var candidates = (await repository.GetRelatedTagsAsync(commit, ct)).
                Select(tag => Version.TryParse(tag.Name, out var v) ? v : null!).
                Where(v => v?.ComponentCount >= 2).     // "1.2" or more.
                OrderByDescending(v => v).
                ToArray();
            if (candidates.Length >= 1)
            {
                version = candidates[0];
                reached.Add(commit, version);
                break;
            }

            var parents = (await LooseConcurrentScope.Default.WhenAll(
                commit.Parents.Select(parentCommitHash => repository.GetCommitAsync(parentCommitHash, ct)))).
                Where(parentCommitHash => parentCommitHash.HasValue).
                Select(parentCommitHash => parentCommitHash!.Value).
                ToArray();
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
                    var v = await LookupVersionLabelRecursiveAsync(repository, pcs[index], reached, ct);
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

    private static async Task<Version> RunLookupVersionLabelAsync(
        PrimitiveRepository repository,
        PrimitiveReference branch, CancellationToken ct)
    {
        var headCommit = await repository.GetCommitAsync(branch, ct);
        return await LookupVersionLabelRecursiveAsync(repository, headCommit!.Value, new(), ct);
    }

    public static async Task<Version> LookupVersionLabelAsync(
        PrimitiveRepository repository,
        bool checkWorkingDirectoryStatus,
        CancellationToken ct)
    {
        // Check if repository has any commits (to handle initial state)
        if (await repository.GetCurrentHeadReferenceAsync(ct) is not { } branch)
        {
            return Version.Default; // Return default version (0.0.1) if no HEAD
        }

        if (checkWorkingDirectoryStatus)
        {
            var (baseVersion, workingDirectoryStatus) = await LooseConcurrentScope.Default.Join(
                // Get the base version from commit tags
                RunLookupVersionLabelAsync(repository, branch, ct),
                // Check working directory status
                repository.GetWorkingDirectoryStatusAsync(ct));

            // If there are modified files, increment the version
            if (workingDirectoryStatus.StagedFiles.Count > 0 ||
                workingDirectoryStatus.UnstagedFiles.Count > 0)
            {
                return IncrementLastVersionComponent(baseVersion);
            }

            var untrackedFiles = await repository.GetUntrackedFilesAsync(workingDirectoryStatus, ct);
            if (untrackedFiles.Count > 0)
            {
                return IncrementLastVersionComponent(baseVersion);
            }

            return baseVersion;
        }
        else
        {
            return await RunLookupVersionLabelAsync(repository, branch, ct);
        }
    }
}
