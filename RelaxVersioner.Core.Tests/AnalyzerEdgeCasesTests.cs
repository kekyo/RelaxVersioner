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
public sealed class AnalyzerEdgeCasesTests
{
    [Test]
    public async Task LookupVersionLabel_EmptyRepository_ReturnsDefaultVersion()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            // Initialize empty git repository
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            Assert.That(version.ToString(), Is.EqualTo("0.0.1"));
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
    public async Task LookupVersionLabel_DetachedHead_ReturnsCorrectVersion()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            // Initialize repository with commits and tag
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v2.1.0");
            
            // Create another commit
            File.WriteAllText(testFile, "modified content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Second commit\"");
            
            // Checkout to detached HEAD state (HEAD~1 points to the tagged commit)
            await TestUtilities.RunGitCommand(tempPath, "checkout HEAD~1");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            Assert.That(version.ToString(), Is.EqualTo("2.1.0"));
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
    public async Task LookupVersionLabel_WithInvalidTags_IgnoresInvalidOnes()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            
            // Add various invalid and valid tags
            await TestUtilities.RunGitCommand(tempPath, "tag invalid-tag");
            await TestUtilities.RunGitCommand(tempPath, "tag v1");  // Single component - should be ignored
            await TestUtilities.RunGitCommand(tempPath, "tag not-a-version");
            await TestUtilities.RunGitCommand(tempPath, "tag v3.2.1");  // Valid
            await TestUtilities.RunGitCommand(tempPath, "tag release-notes");
            await TestUtilities.RunGitCommand(tempPath, "tag v2.5.0");  // Valid but smaller
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should pick the largest valid version (v3.2.1)
            Assert.That(version.ToString(), Is.EqualTo("3.2.1"));
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
    public async Task LookupVersionLabel_WithOrphanBranch_ReturnsDefaultVersion()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            // Create main branch with tagged commit
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.0");
            
            // Create orphan branch
            await TestUtilities.RunGitCommand(tempPath, "checkout --orphan orphan-branch");
            await TestUtilities.RunGitCommand(tempPath, "rm -f test.txt");
            
            var orphanFile = Path.Combine(tempPath, "orphan.txt");
            File.WriteAllText(orphanFile, "orphan content");
            await TestUtilities.RunGitCommand(tempPath, "add orphan.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Orphan commit\"");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Orphan branch should return default version
            Assert.That(version.ToString(), Is.EqualTo("0.0.1"));
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
    public async Task LookupVersionLabel_DeepHistory_PerformsCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "initial");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.0");
            
            // Create deep history (50 commits)
            for (int i = 1; i <= 50; i++)
            {
                File.WriteAllText(testFile, $"content {i}");
                await TestUtilities.RunGitCommand(tempPath, "add test.txt");
                await TestUtilities.RunGitCommand(tempPath, $"commit -m \"Commit {i}\"");
            }
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should be v1.0.0 + 50 increments = v1.0.50
            Assert.That(version.ToString(), Is.EqualTo("1.0.50"));
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
    public async Task LookupVersionLabel_MultipleTagsOnSameCommit_ChoosesLargest()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Tagged commit\"");
            
            // Add multiple tags to the same commit
            await TestUtilities.RunGitCommand(tempPath, "tag v1.5.0");
            await TestUtilities.RunGitCommand(tempPath, "tag v2.0.0");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.9.9");
            await TestUtilities.RunGitCommand(tempPath, "tag v2.0.1");  // This should be the largest
            await TestUtilities.RunGitCommand(tempPath, "tag v1.10.0");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
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
    public async Task LookupVersionLabel_WithWorkingDirectoryChanges_IncrementsCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.2.3.4");  // 4-component version
            
            // Test clean state
            using (var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath))
            {
                var version = await Analyzer.LookupVersionLabelAsync(repository, true, default);
                Assert.That(version.ToString(), Is.EqualTo("1.2.3.4"));
            }
            
            // Test with unstaged changes
            File.WriteAllText(testFile, "modified");
            using (var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath))
            {
                var version = await Analyzer.LookupVersionLabelAsync(repository, true, default);
                Assert.That(version.ToString(), Is.EqualTo("1.2.3.5"));  // Revision incremented
            }
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
    public async Task LookupVersionLabel_DifferentVersionFormats_ParsesCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            
            // Test 2-component version
            File.WriteAllText(testFile, "content1");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Commit 1\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v5.10");
            
            File.WriteAllText(testFile, "content2");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Commit 2\"");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should be v5.10 + 1 = v5.11
            Assert.That(version.ToString(), Is.EqualTo("5.11"));
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