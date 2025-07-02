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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RelaxVersioner;

[NonParallelizable]
public sealed class AnalyzerPerformanceTests
{
    [Test]
    [Order(6)]
    public async Task LookupVersionLabel_LargeHistoryPerformance_CompletesInReasonableTime()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "initial");
            await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
            await TestUtilities.RunGitCommandAsync(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "tag v1.0.0");
            
            // Create large history (200 commits)
            for (int i = 1; i <= 200; i++)
            {
                File.WriteAllText(testFile, $"content {i}");
                await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
                await TestUtilities.RunGitCommandAsync(tempPath, $"commit -m \"Commit {i}\"");
                
                // Add some tags sporadically - but not on the last commit
                if (i % 50 == 0 && i < 200)
                {
                    await TestUtilities.RunGitCommandAsync(tempPath, $"tag v1.{i / 50}.0");
                }
                // Add the final tag to the 199th commit (not HEAD)
                else if (i == 199)
                {
                    await TestUtilities.RunGitCommandAsync(tempPath, "tag v1.4.0");
                }
            }
            
            var stopwatch = Stopwatch.StartNew();
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            stopwatch.Stop();
            
            // Should complete within reasonable time (5 seconds)
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5000), 
                "Performance test took too long");
            
            // Should find v1.4.0 (last tag) and increment to v1.4.1
            Assert.That(version.ToString(), Is.EqualTo("1.4.1"));
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
    [Order(2)]
    public async Task LookupVersionLabel_ManyTags_PerformsCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
            await TestUtilities.RunGitCommandAsync(tempPath, "commit -m \"Initial commit\"");
            
            // Add many tags to the same commit
            for (int i = 1; i <= 100; i++)
            {
                await TestUtilities.RunGitCommandAsync(tempPath, $"tag v{i}.0.0");
                await TestUtilities.RunGitCommandAsync(tempPath, $"tag invalid-tag-{i}");  // Invalid tags
            }
            
            var stopwatch = Stopwatch.StartNew();
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            stopwatch.Stop();
            
            // Should complete within reasonable time
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(3000), 
                "Many tags test took too long");
            
            // Should find the largest version (v100.0.0)
            Assert.That(version.ToString(), Is.EqualTo("100.0.0"));
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
    [Order(5)]
    public async Task LookupVersionLabel_ComplexMergeTree_PerformsCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "base");
            await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
            await TestUtilities.RunGitCommandAsync(tempPath, "commit -m \"Base commit\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "tag v1.0.0");
            
            // Create complex merge tree with many branches
            for (int i = 1; i <= 20; i++)
            {
                await TestUtilities.RunGitCommandAsync(tempPath, $"checkout -b branch{i}");
                File.WriteAllText(testFile, $"branch {i} content");
                await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
                await TestUtilities.RunGitCommandAsync(tempPath, $"commit -m \"Branch {i} commit\"");
                
                // Add tags to some branches
                if (i % 5 == 0)
                {
                    await TestUtilities.RunGitCommandAsync(tempPath, $"tag v{i / 5 + 1}.0.0");
                }
                
                // Merge back to main
                await TestUtilities.RunGitCommandAsync(tempPath, "checkout main");
                await TestUtilities.RunGitCommandAsync(tempPath, $"merge branch{i} --no-ff --no-edit");
            }
            
            var stopwatch = Stopwatch.StartNew();
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            stopwatch.Stop();
            
            // Should complete within reasonable time
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(10000), 
                "Complex merge tree test took too long");
            
            // Should find v5.0.0 (highest tag from branch20) and increment
            Assert.That(version.ToString(), Is.EqualTo("5.0.1"));
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
    [Order(1)]
    public async Task LookupVersionLabel_WithCancellation_RespondsToToken()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
            await TestUtilities.RunGitCommandAsync(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "tag v1.0.0");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            using var cts = new CancellationTokenSource();
            
            // Cancel immediately
            cts.Cancel();
            
            // Should respect cancellation token
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await Analyzer.LookupVersionLabelAsync(repository, false, cts.Token));
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
    [Order(3)]
    public async Task LookupVersionLabel_ConcurrentAccess_HandledCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
            await TestUtilities.RunGitCommandAsync(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "tag v1.0.0");
            
            // Create multiple concurrent tasks
            var tasks = new Task<Version>[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
                    return await Analyzer.LookupVersionLabelAsync(repository, false, default);
                });
            }
            
            var stopwatch = Stopwatch.StartNew();
            var results = await Task.WhenAll(tasks);
            stopwatch.Stop();
            
            // All results should be the same
            var expectedVersion = "1.0.0";
            foreach (var result in results)
            {
                Assert.That(result.ToString(), Is.EqualTo(expectedVersion));
            }
            
            // Should complete within reasonable time
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5000), 
                "Concurrent access test took too long");
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
    [Order(4)]
    public async Task LookupVersionLabel_WithWorkingDirectoryCheck_PerformsCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
            await TestUtilities.RunGitCommandAsync(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "tag v1.0.0");
            
            // Create many untracked files
            for (int i = 1; i <= 100; i++)
            {
                var untrackedFile = Path.Combine(tempPath, $"untracked{i}.txt");
                File.WriteAllText(untrackedFile, $"untracked content {i}");
            }
            
            var stopwatch = Stopwatch.StartNew();
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, true, default);
            
            stopwatch.Stop();
            
            // Should complete within reasonable time even with many untracked files
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(10000), 
                "Working directory check with many files took too long");
            
            // Should increment due to untracked files
            Assert.That(version.ToString(), Is.EqualTo("1.0.1"));
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