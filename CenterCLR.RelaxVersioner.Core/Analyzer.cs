﻿////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

#nullable enable

using LibGit2Sharp;
using System.Collections.Generic;
using System.Linq;

namespace RelaxVersioner;

internal static class Analyzer
{
    private readonly struct TargetCommit
    {
        public readonly int StartDepth;
        public readonly Commit Commit;

        public TargetCommit(int startDepth, Commit commit)
        {
            this.StartDepth = startDepth;
            this.Commit = commit;
        }

        public override string ToString() =>
            $"StartDepth={this.StartDepth}, {this.Commit}";
    }

    public static Version LookupVersionLabel(
        Branch targetBranch,
        Dictionary<string, Tag[]> tagsDictionary)
    {
        var topCommit = targetBranch?.Commits?.FirstOrDefault();
        if (topCommit == null)
        {
            return Version.Default;
        }

        var reached = new HashSet<string>();
        var scheduled = new Stack<TargetCommit>();
        scheduled.Push(new TargetCommit(0, topCommit));

        var mainDepth = 0;

        while (scheduled.Count >= 1)
        {
            // Extract an analysis commit.
            var entry = scheduled.Pop();
            
            var currentCommit = entry.Commit;
            var currentDepth = entry.StartDepth;

            while (true)
            {
                // Rejoined parent branch.
                if (!reached.Add(currentCommit.Sha))
                {
                    break;
                }

                // If found be applied tags at this commit:
                if (tagsDictionary.TryGetValue(currentCommit.Sha, out var tags))
                {
                    var filteredTags = tags.
                        Select(tag => Version.TryParse(tag.GetFriendlyName(), out var version) ? (Version?)version : null).
                        Where(version => version.HasValue).
                        Select(version => Utilities.IncrementLastVersionComponent(version!.Value, currentDepth)).
                        ToArray();

                    // Found first valid tag.
                    if (filteredTags.Length >= 1)
                    {
                        return filteredTags[0];
                    }
                }

                // Found parents.
                if ((currentCommit.Parents?.ToArray() is { Length: >= 1 } parents))
                {
                    // Dive parent commit.
                    currentDepth++;

                    // Next commit is a primary parent.
                    currentCommit = parents[0];

                    // Enqueue analysis scheduling if it has multiple parents.
                    foreach (var parentCommit in parents.Skip(1))
                    {
                        scheduled.Push(new TargetCommit(currentDepth, parentCommit));
                    }
                }
                // Bottom of branch.
                else
                {
                    // Save depth if it's on boarding the main branch.
                    if (mainDepth == 0)
                    {
                        mainDepth = currentDepth;
                    }
                    
                    // Goes to next scheduled commit.
                    break;
                }
            }
        }

        // Finally got the main branch depth.
        return Utilities.IncrementLastVersionComponent(Version.Default, mainDepth);
    }
}
