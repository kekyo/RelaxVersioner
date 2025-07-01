////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using GitReader;
using GitReader.Primitive;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RelaxVersioner;

[Parallelizable(ParallelScope.All)]
public sealed class AnalyzerMergeScenarioTests
{
    [Test]
    public async Task LookupVersionLabel_SimpleMerge_ChoosesLargerVersion()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            
            // Create initial commit
            File.WriteAllText(testFile, "initial");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.0");
            
            // Create feature branch with higher version tag
            await TestUtilities.RunGitCommand(tempPath, "checkout -b feature");
            File.WriteAllText(testFile, "feature content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Feature commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v2.0.0");
            
            // Switch back to main and create parallel commit
            await TestUtilities.RunGitCommand(tempPath, "checkout main");
            var mainFile = Path.Combine(tempPath, "main.txt");
            File.WriteAllText(mainFile, "main content");
            await TestUtilities.RunGitCommand(tempPath, "add main.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Main commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.5.0");  // Lower version
            
            // Merge feature branch
            await TestUtilities.RunGitCommand(tempPath, "merge feature --no-ff --no-edit");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should choose larger version (v2.0.0) and increment to v2.0.1
            Assert.That(version.ToString(), Is.EqualTo("2.0.1"));
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }

    [Test]
    public async Task LookupVersionLabel_MultipleMerges_ChoosesLargestVersion()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            
            // Create initial commit
            File.WriteAllText(testFile, "initial");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.0");
            
            // Create branch A with version 2.0.0
            await TestUtilities.RunGitCommand(tempPath, "checkout -b branchA");
            File.WriteAllText(testFile, "branch A content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Branch A commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v2.0.0");
            
            // Create branch B with version 3.0.0 (should be largest)
            await TestUtilities.RunGitCommand(tempPath, "checkout main");
            await TestUtilities.RunGitCommand(tempPath, "checkout -b branchB");
            var branchBFile = Path.Combine(tempPath, "branchB.txt");
            File.WriteAllText(branchBFile, "branch B content");
            await TestUtilities.RunGitCommand(tempPath, "add branchB.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Branch B commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v3.0.0");
            
            // Create branch C with version 1.5.0 (smaller)
            await TestUtilities.RunGitCommand(tempPath, "checkout main");
            await TestUtilities.RunGitCommand(tempPath, "checkout -b branchC");
            var branchCFile = Path.Combine(tempPath, "branchC.txt");
            File.WriteAllText(branchCFile, "branch C content");
            await TestUtilities.RunGitCommand(tempPath, "add branchC.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Branch C commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.5.0");
            
            // Merge all branches into main
            await TestUtilities.RunGitCommand(tempPath, "checkout main");
            await TestUtilities.RunGitCommand(tempPath, "merge branchA --no-ff --no-edit");
            await TestUtilities.RunGitCommand(tempPath, "merge branchB --no-ff --no-edit");
            await TestUtilities.RunGitCommand(tempPath, "merge branchC --no-ff --no-edit");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should choose largest version (v3.0.0) and increment to v3.0.2
            Assert.That(version.ToString(), Is.EqualTo("3.0.2"));
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }

    [Test]
    public async Task LookupVersionLabel_DeepMergeHistory_PerformsCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            
            // Create base commit
            File.WriteAllText(testFile, "base");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Base commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.0");
            
            // Create deep branch structure with nested merges
            for (int i = 1; i <= 5; i++)
            {
                await TestUtilities.RunGitCommand(tempPath, $"checkout -b branch{i}");
                File.WriteAllText(testFile, $"branch {i} content");
                await TestUtilities.RunGitCommand(tempPath, "add test.txt");
                await TestUtilities.RunGitCommand(tempPath, $"commit -m \"Branch {i} commit\"");
                
                // Only tag some branches
                if (i == 3)
                    await TestUtilities.RunGitCommand(tempPath, "tag v2.5.0");
                else if (i == 5)
                    await TestUtilities.RunGitCommand(tempPath, "tag v3.1.0");  // Should be largest
                
                // Merge back to main
                await TestUtilities.RunGitCommand(tempPath, "checkout main");
                await TestUtilities.RunGitCommand(tempPath, $"merge branch{i} --no-ff --no-edit");
            }
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should find v3.1.0 and increment to v3.1.1
            Assert.That(version.ToString(), Is.EqualTo("3.1.1"));
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }

    [Test]
    public async Task LookupVersionLabel_NoMergeCommitsWithTags_IncrementsFromBase()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            
            // Create base commit with tag
            File.WriteAllText(testFile, "base");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Base commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.0");
            
            // Create feature branch without merge
            await TestUtilities.RunGitCommand(tempPath, "checkout -b feature");
            File.WriteAllText(testFile, "feature");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Feature commit\"");
            
            // Add more commits without tags
            for (int i = 1; i <= 3; i++)
            {
                File.WriteAllText(testFile, $"feature {i}");
                await TestUtilities.RunGitCommand(tempPath, "add test.txt");
                await TestUtilities.RunGitCommand(tempPath, $"commit -m \"Feature commit {i}\"");
            }
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should be v1.0.0 + 4 increments = v1.0.4
            Assert.That(version.ToString(), Is.EqualTo("1.0.4"));
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }

    [Test]
    public async Task LookupVersionLabel_OctopusMerge_ChoosesLargestVersion()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            
            // Create initial commit
            File.WriteAllText(testFile, "initial");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.0");
            
            // Create multiple branches with different versions
            var branches = new[] { ("branchA", "v2.1.0"), ("branchB", "v1.9.0"), ("branchC", "v2.5.0") };
            
            foreach (var (branchName, tagName) in branches)
            {
                await TestUtilities.RunGitCommand(tempPath, $"checkout -b {branchName}");
                var branchFile = Path.Combine(tempPath, $"{branchName}.txt");
                File.WriteAllText(branchFile, $"{branchName} content");
                await TestUtilities.RunGitCommand(tempPath, $"add {branchName}.txt");
                await TestUtilities.RunGitCommand(tempPath, $"commit -m \"{branchName} commit\"");
                await TestUtilities.RunGitCommand(tempPath, $"tag {tagName}");
                await TestUtilities.RunGitCommand(tempPath, "checkout main");
            }
            
            // Perform octopus merge (merge multiple branches at once)
            await TestUtilities.RunGitCommand(tempPath, "merge branchA branchB branchC --no-ff --no-edit");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should choose largest version (v2.5.0) and increment to v2.5.1
            Assert.That(version.ToString(), Is.EqualTo("2.5.1"));
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }

    [Test]
    public async Task LookupVersionLabel_NestedMerges_HandlesRecursion()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            
            // Create base
            File.WriteAllText(testFile, "base");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Base commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.0");
            
            // Create branch1 with tag
            await TestUtilities.RunGitCommand(tempPath, "checkout -b branch1");
            File.WriteAllText(testFile, "branch1");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Branch1 commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v2.0.0");
            
            // Create branch2 from branch1 with higher tag
            await TestUtilities.RunGitCommand(tempPath, "checkout -b branch2");
            var branch2File = Path.Combine(tempPath, "branch2.txt");
            File.WriteAllText(branch2File, "branch2");
            await TestUtilities.RunGitCommand(tempPath, "add branch2.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Branch2 commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v3.0.0");  // Highest
            
            // Merge branch2 back to branch1
            await TestUtilities.RunGitCommand(tempPath, "checkout branch1");
            await TestUtilities.RunGitCommand(tempPath, "merge branch2 --no-ff --no-edit");
            
            // Merge branch1 to main
            await TestUtilities.RunGitCommand(tempPath, "checkout main");
            await TestUtilities.RunGitCommand(tempPath, "merge branch1 --no-ff --no-edit");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should find v3.0.0 and increment to v3.0.2 (2 merge commits)
            Assert.That(version.ToString(), Is.EqualTo("3.0.2"));
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }
} 