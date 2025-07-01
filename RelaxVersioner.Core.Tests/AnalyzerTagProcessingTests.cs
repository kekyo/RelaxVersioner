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
public sealed class AnalyzerTagProcessingTests
{
    [Test]
    public async Task LookupVersionLabel_TagWithDifferentSeparators_ParsesCorrectly()
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
            
            // Add tags with different separators (should be parseable by Version.TryParse)
            await TestUtilities.RunGitCommand(tempPath, "tag v2-1-0");  // dash separator
            await TestUtilities.RunGitCommand(tempPath, "tag v1_5_3");  // underscore separator
            await TestUtilities.RunGitCommand(tempPath, "tag v3,2,1");  // comma separator
            await TestUtilities.RunGitCommand(tempPath, "tag v2/0/5");  // slash separator
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should pick the largest valid version (v3,2,1 = 3.2.1)
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
    public async Task LookupVersionLabel_TagWithVPrefix_ParsesCorrectly()
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
            
            // Add tags with and without 'v' prefix
            await TestUtilities.RunGitCommand(tempPath, "tag 2.1.0");    // without 'v'
            await TestUtilities.RunGitCommand(tempPath, "tag v3.0.0");   // with 'v'
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should pick the largest version (v3.0.0)
            Assert.That(version.ToString(), Is.EqualTo("3.0.0"));
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
    public async Task LookupVersionLabel_TagsWithInvalidVersions_IgnoresInvalid()
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
            
            // Add invalid version tags that should be ignored
            await TestUtilities.RunGitCommand(tempPath, "tag v1.2.3.4.5");     // Too many components
            await TestUtilities.RunGitCommand(tempPath, "tag v-1.2.3");        // Negative number
            await TestUtilities.RunGitCommand(tempPath, "tag v1.2.3.beta");    // Non-numeric component
            await TestUtilities.RunGitCommand(tempPath, "tag v1.2.3-alpha");   // Suffix
            await TestUtilities.RunGitCommand(tempPath, "tag v");              // Empty version
            await TestUtilities.RunGitCommand(tempPath, "tag v1.2.3 ");       // Whitespace
            await TestUtilities.RunGitCommand(tempPath, "tag v70000.2.3");     // Exceeds ushort.MaxValue
            
            // Add one valid tag
            await TestUtilities.RunGitCommand(tempPath, "tag v1.5.2");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should use the only valid tag
            Assert.That(version.ToString(), Is.EqualTo("1.5.2"));
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
    public async Task LookupVersionLabel_SingleComponentTag_IsIgnored()
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
            
            // Add single component tags (should be ignored as per Analyzer code: ComponentCount >= 2)
            await TestUtilities.RunGitCommand(tempPath, "tag v5");
            await TestUtilities.RunGitCommand(tempPath, "tag v10");
            
            File.WriteAllText(testFile, "modified");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Second commit\"");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Since single component tags are ignored, should get default incremented version
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
    public async Task LookupVersionLabel_TagsOnDifferentCommits_ChoosesCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            await TestUtilities.RunGitCommand(tempPath, "init");
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            var testFile = Path.Combine(tempPath, "test.txt");
            
            // First commit with tag
            File.WriteAllText(testFile, "content1");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"First commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.0.0");
            
            // Second commit with tag
            File.WriteAllText(testFile, "content2");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Second commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v2.0.0");
            
            // Third commit without tag
            File.WriteAllText(testFile, "content3");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Third commit\"");
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            // Should find v2.0.0 and increment to v2.0.1
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
    public async Task LookupVersionLabel_FourComponentVersions_HandledCorrectly()
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
            
            // Add different 4-component versions
            await TestUtilities.RunGitCommand(tempPath, "tag v1.2.3.4");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.2.3.10");  // Higher revision
            await TestUtilities.RunGitCommand(tempPath, "tag v1.2.4.1");   // Higher build
            await TestUtilities.RunGitCommand(tempPath, "tag v1.3.0.0");   // Higher minor
            await TestUtilities.RunGitCommand(tempPath, "tag v2.0.0.0");   // Higher major - should be largest
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            Assert.That(version.ToString(), Is.EqualTo("2.0.0.0"));
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
    public async Task LookupVersionLabel_MaxValueVersionComponents_HandledCorrectly()
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
            
            // Test with maximum ushort values
            await TestUtilities.RunGitCommand(tempPath, "tag v65535.65535.65535");  // ushort.MaxValue
            await TestUtilities.RunGitCommand(tempPath, "tag v1.2.3");              // Smaller version
            
            using var repository = await Repository.Factory.OpenPrimitiveAsync(tempPath);
            var version = await Analyzer.LookupVersionLabelAsync(repository, false, default);
            
            Assert.That(version.ToString(), Is.EqualTo("65535.65535.65535"));
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