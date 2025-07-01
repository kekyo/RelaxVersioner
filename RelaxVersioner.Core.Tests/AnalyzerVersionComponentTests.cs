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
public sealed class AnalyzerVersionComponentTests
{
    [Test]
    public async Task LookupVersionLabel_IncrementMajorOnly_IncrementsCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.RunGitCommand(tempPath, "init");
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v5");  // Major only (but should be ignored due to ComponentCount < 2)
            
            // Create another commit to increment from default
            File.WriteAllText(testFile, "modified");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Second commit\"");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Since single component tags are ignored, should get default incremented
            Assert.That(version.ToString(), Is.EqualTo("0.0.2"));
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
    public async Task LookupVersionLabel_IncrementTwoComponent_IncrementsMinor()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.RunGitCommand(tempPath, "init");
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v3.5");  // Two components
            
            File.WriteAllText(testFile, "modified");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Second commit\"");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should increment minor: 3.5 -> 3.6
            Assert.That(version.ToString(), Is.EqualTo("3.6"));
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
    public async Task LookupVersionLabel_IncrementThreeComponent_IncrementsBuild()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.RunGitCommand(tempPath, "init");
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v2.1.7");  // Three components
            
            File.WriteAllText(testFile, "modified");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Second commit\"");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should increment build: 2.1.7 -> 2.1.8
            Assert.That(version.ToString(), Is.EqualTo("2.1.8"));
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
    public async Task LookupVersionLabel_IncrementFourComponent_IncrementsRevision()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.RunGitCommand(tempPath, "init");
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.2.3.4");  // Four components
            
            File.WriteAllText(testFile, "modified");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Second commit\"");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should increment revision: 1.2.3.4 -> 1.2.3.5
            Assert.That(version.ToString(), Is.EqualTo("1.2.3.5"));
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
    public async Task LookupVersionLabel_ZeroComponentValues_HandledCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.RunGitCommand(tempPath, "init");
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v0.0.0.0");  // All zeros
            
            File.WriteAllText(testFile, "modified");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Second commit\"");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should increment revision: 0.0.0.0 -> 0.0.0.1
            Assert.That(version.ToString(), Is.EqualTo("0.0.0.1"));
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
    public async Task LookupVersionLabel_MultipleIncrementsFromSameBase_AccumulatesCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.RunGitCommand(tempPath, "init");
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.0");
            
            // Create 10 additional commits
            for (int i = 1; i <= 10; i++)
            {
                File.WriteAllText(testFile, $"content {i}");
                await TestUtilities.RunGitCommand(tempPath, "add test.txt");
                await TestUtilities.RunGitCommand(tempPath, $"commit -m \"Commit {i}\"");
            }
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should be v1.0.0 + 10 increments = v1.0.10
            Assert.That(version.ToString(), Is.EqualTo("1.0.10"));
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
    public async Task LookupVersionLabel_MixedComponentCounts_ChoosesLargestCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.RunGitCommand(tempPath, "init");
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            
            // Add tags with different component counts
            await TestUtilities.RunGitCommand(tempPath, "tag v2.0");          // 2 components: 2.0.0.0
            await TestUtilities.RunGitCommand(tempPath, "tag v1.9.9");        // 3 components: 1.9.9.0
            await TestUtilities.RunGitCommand(tempPath, "tag v2.0.0.5");      // 4 components: 2.0.0.5 - should be largest
            await TestUtilities.RunGitCommand(tempPath, "tag v1.9.9.9");      // 4 components: 1.9.9.9
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should choose v2.0.0.5 (largest when compared as 4-component versions)
            Assert.That(version.ToString(), Is.EqualTo("2.0.0.5"));
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
    public async Task LookupVersionLabel_EdgeCaseVersionComparisons_HandledCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.RunGitCommand(tempPath, "init");
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            
            // Test edge cases in version comparison
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.10");       // 1.0.10.0
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.9.1");      // 1.0.9.1 (smaller build but has revision)
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.10.0");     // 1.0.10.0 - should be largest
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.2.99");     // 1.0.2.99 (much smaller build)
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // v1.0.10.0 should be largest (1.0.10 becomes 1.0.10.0 when compared)
            Assert.That(version.ToString(), Is.EqualTo("1.0.10.0"));
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
    public async Task LookupVersionLabel_LargeComponentValues_HandledCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.RunGitCommand(tempPath, "init");
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v65534.65534.65534.65534");  // Near ushort.MaxValue
            
            File.WriteAllText(testFile, "modified");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Second commit\"");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should increment revision: 65534.65534.65534.65534 -> 65534.65534.65534.65535
            Assert.That(version.ToString(), Is.EqualTo("65534.65534.65534.65535"));
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