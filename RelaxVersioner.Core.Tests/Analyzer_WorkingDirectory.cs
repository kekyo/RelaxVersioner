////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using GitReader;
using GitReader.Structures;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RelaxVersioner;

[Parallelizable(ParallelScope.All)]
public sealed class Analyzer_WorkingDirectory
{
    [Test]
    public async Task LookupVersionLabelWithWorkingDirectoryChanges()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{System.Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            // Initialize git repository
            await TestUtilities.RunGitCommand(tempPath, "init");
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            // Create initial commit with version tag
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "initial content");
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            await TestUtilities.RunGitCommand(tempPath, "commit -m \"Initial commit\"");
            await TestUtilities.RunGitCommand(tempPath, "tag v1.2.3");
            
            // Test 1: Clean working directory - should return tagged version
            using (var repository = await Repository.Factory.OpenStructureAsync(tempPath))
            {
                var version = await Analyzer.LookupVersionLabelAsync(repository, default);
                Assert.That(version.ToString(), Is.EqualTo("1.2.3"));
            }
            
            // Test 2: Modified file (unstaged) - should increment version
            File.WriteAllText(testFile, "modified content");
            using (var repository = await Repository.Factory.OpenStructureAsync(tempPath))
            {
                var version = await Analyzer.LookupVersionLabelAsync(repository, default);
                Assert.That(version.ToString(), Is.EqualTo("1.2.4"));
            }
            
            // Test 3: Staged file - should increment version
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            using (var repository = await Repository.Factory.OpenStructureAsync(tempPath))
            {
                var version = await Analyzer.LookupVersionLabelAsync(repository, default);
                Assert.That(version.ToString(), Is.EqualTo("1.2.4"));
            }
            
            // Test 4: Clean up staged file and test untracked file (should not increment)
            await TestUtilities.RunGitCommand(tempPath, "reset HEAD test.txt");
            await TestUtilities.RunGitCommand(tempPath, "checkout -- test.txt");
            var untrackedFile = Path.Combine(tempPath, "untracked.txt");
            File.WriteAllText(untrackedFile, "untracked content");
            using (var repository = await Repository.Factory.OpenStructureAsync(tempPath))
            {
                var version = await Analyzer.LookupVersionLabelAsync(repository, default);
                Assert.That(version.ToString(), Is.EqualTo("1.2.3")); // Should not increment for untracked files
            }
        }
        finally
        {
            // Clean up
            if (Directory.Exists(tempPath))
            {
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }
    
    [Test]
    public async Task LookupVersionLabelWithInitialRepository()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTest_{System.Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(tempPath);
            
            // Initialize empty git repository
            await TestUtilities.RunGitCommand(tempPath, "init");
            await TestUtilities.RunGitCommand(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommand(tempPath, "config user.name \"Test User\"");
            
            // Test 1: Initial repository with no commits and no files - should return default version
            using (var repository = await Repository.Factory.OpenStructureAsync(tempPath))
            {
                var version = await Analyzer.LookupVersionLabelAsync(repository, default);
                Assert.That(version.ToString(), Is.EqualTo("0.0.1"));
            }
            
            // Test 2: Initial repository with untracked file - should still return default version
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "content");
            using (var repository = await Repository.Factory.OpenStructureAsync(tempPath))
            {
                var version = await Analyzer.LookupVersionLabelAsync(repository, default);
                Assert.That(version.ToString(), Is.EqualTo("0.0.1"));
            }
            
            // Test 3: Initial repository with staged file (but no commits) - should still return default version
            await TestUtilities.RunGitCommand(tempPath, "add test.txt");
            using (var repository = await Repository.Factory.OpenStructureAsync(tempPath))
            {
                var version = await Analyzer.LookupVersionLabelAsync(repository, default);
                Assert.That(version.ToString(), Is.EqualTo("0.0.1"));
            }
            
            // Test 4: Initial repository with unstaged changes after staging - should still return default version
            // First modify the staged file to create unstaged changes
            File.WriteAllText(testFile, "modified content");
            using (var repository = await Repository.Factory.OpenStructureAsync(tempPath))
            {
                var version = await Analyzer.LookupVersionLabelAsync(repository, default);
                Assert.That(version.ToString(), Is.EqualTo("0.0.1"));
            }
            
            // Test 5: Add another file and stage it alongside unstaged changes
            var testFile2 = Path.Combine(tempPath, "test2.txt");
            File.WriteAllText(testFile2, "second file content");
            await TestUtilities.RunGitCommand(tempPath, "add test2.txt");
            using (var repository = await Repository.Factory.OpenStructureAsync(tempPath))
            {
                var version = await Analyzer.LookupVersionLabelAsync(repository, default);
                Assert.That(version.ToString(), Is.EqualTo("0.0.1"));
            }
        }
        finally
        {
            // Clean up
            if (Directory.Exists(tempPath))
            {
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }
} 