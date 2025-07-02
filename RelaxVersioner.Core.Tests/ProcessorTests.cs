////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;

namespace RelaxVersioner;

[Parallelizable(ParallelScope.All)]
public sealed class ProcessorTests
{
    [TestCase("Text", "expected_Text.txt", "{versionLabel}")]
    [TestCase("C#", "expected_CSharp.txt", "")]
    [TestCase("F#", "expected_FSharp.txt", "")]
    [TestCase("VB", "expected_VB.txt", "")]
    [TestCase("C++/CLI", "expected_CPlusPlusCLI.txt", "")]
    public async Task RunAsync_WithValidRepository_ReturnsSuccessfulResult(string language, string expectedFileName, string textFormat)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerProcessorTest_{Guid.NewGuid():N}");

        try
        {
            // Setup test repository
            Directory.CreateDirectory(tempPath);

            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.name \"Test User\"");

            // Create initial commit
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "test content");
            await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
            await TestUtilities.RunGitCommandWithOutputAsync(tempPath, "commit -m \"Initial commit\"");

            // Create Processor with logger
            using var outWriter = new StringWriter();
            using var errorWriter = new StringWriter();
            var logger = Logger.Create("ProcessorTest", LogImportance.Normal, outWriter, errorWriter, errorWriter);
            var processor = new Processor(logger);

            // Setup ProcessorContext for actual output
            var outputPath = Path.Combine(tempPath, $"version_output_{language.Replace("/", "_").Replace("#", "Sharp")}.txt");
            var context = new ProcessorContext
            {
                ProjectDirectory = tempPath,
                Language = language,
                OutputPath = outputPath,
                TextFormat = string.IsNullOrEmpty(textFormat) ? "{versionLabel}" : textFormat,
                Namespace = "Test.Namespace",
                IsDryRun = false // Perform actual file operations
            };
            
            //-------------------------------------------------------------

            // Execute RunAsync
            var result = await processor.RunAsync(context, CancellationToken.None);
            
            //-------------------------------------------------------------

            // Load output file was created
            Assert.That(File.Exists(outputPath), Is.True, "Output file should be created");
            var outputContent = await TestUtilities.ReadAllTextAsync(outputPath);

            // Load expected content from asset file
            var expectedPath = Path.Combine("artifacts", "WithValidRepository_ReturnsSuccessfulResult", expectedFileName);
            Assert.That(File.Exists(expectedPath), Is.True, $"Expected asset file should exist: {expectedPath}");
            var expectedContent = await TestUtilities.ReadAllTextAsync(expectedPath);
            
            //-------------------------------------------------------------

            // Get actual values for placeholder replacement
            var placeholders = new Dictionary<string, string>
            {
                [PlaceholderKeys.VERSION] = result.Version.ToString(),
                [PlaceholderKeys.ASSEMBLY_VERSION] = result.Version.ToString(),
                [PlaceholderKeys.FILE_VERSION] = result.SafeVersion.ToString(),
                [PlaceholderKeys.APPLICATION_VERSION] = result.EpochIntDateVersion,
                [PlaceholderKeys.APPLICATION_DISPLAY_VERSION] = result.ShortVersion,
                [PlaceholderKeys.COMMIT_ID] = result.CommitId,
                [PlaceholderKeys.BRANCH] = result.Branch ?? "main",
                [PlaceholderKeys.TAGS] = string.Join(",", result.Tags),
                [PlaceholderKeys.AUTHOR] = result.Author,
                [PlaceholderKeys.COMMITTER] = result.Committer,
                [PlaceholderKeys.SUBJECT] = result.Subject ?? "",
                [PlaceholderKeys.BODY] = result.Body ?? "",
                [PlaceholderKeys.DATE] = result.Date.ToString("dddd, dd MMMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture).Replace(":", "")
            };

            // Replace placeholders in expected content and normalize both for comparison
            expectedContent = PlaceholderHelper.ReplacePlaceholders(expectedContent, placeholders);
            
            var normalizedExpectedContent = PlaceholderHelper.NormalizeOutputForComparison(expectedContent);
            var normalizedOutputContent = PlaceholderHelper.NormalizeOutputForComparison(outputContent);

            // Compare output with expected content
            Assert.That(normalizedOutputContent, Is.EqualTo(normalizedExpectedContent), 
                $"Output for {language} should match expected content from {expectedFileName}");

            // Verify result properties with specific expected values
            Assert.That(result.Version.ToString(), Is.EqualTo("0.0.1"), "Version should be default 0.0.1");
            Assert.That(result.ShortVersion, Is.EqualTo("0.0.1"), "Short version should match");
            Assert.That(result.CommitId, Has.Length.EqualTo(40), "CommitId should be a 40-character hash");
            Assert.That(result.CommitId, Does.Match("^[a-f0-9]{40}$"), "CommitId should be a valid SHA-1 hash");
            Assert.That(result.Author, Is.EqualTo("Test User <test@example.com>"), "Author should match configured user");
            Assert.That(result.Committer, Is.EqualTo("Test User <test@example.com>"), "Committer should match configured user");
            Assert.That(result.Subject, Is.EqualTo("Initial commit"), "Subject should match commit message");
            Assert.That(result.Body, Is.EqualTo(string.Empty), "Body should be empty for single-line commit message");
            Assert.That(result.Branch, Is.EqualTo("main"), "Branch should be main");
            Assert.That(result.Tags, Is.Empty, "No tags should be present initially");
            Assert.That(result.Date, Is.LessThanOrEqualTo(DateTimeOffset.Now), "Date should be recent");
            Assert.That(result.Date, Is.GreaterThan(DateTimeOffset.Now.AddMinutes(-1)), "Date should be within last minute");
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }

    [TestCase("{versionLabel}", "0.0.1")]
    [TestCase("{shortVersion}", "0.0.1")]
    [TestCase("{versionLabel}-{commitId}", "0.0.1-[COMMIT_ID_PLACEHOLDER]")]
    [TestCase("Version: {versionLabel}, Branch: {branches}", "Version: 0.0.1, Branch: main")]
    [TestCase("{author} committed on {branches}", "Test User <test@example.com> committed on main")]
    [TestCase("{commit.Subject}", "")]
    [TestCase("{commit.Body}", "")]
    [TestCase("Complex: {versionLabel}.{safeVersion}.{branches}", "Complex: 0.0.1.[SAFE_VERSION_PLACEHOLDER].main")]
    [TestCase("{tags}", "")]
    [TestCase("Generated date: {generated:yyyy-MM-dd}", "Generated date: GENERATED_DATE_COMMENT_PLACEHOLDER")]
    public async Task RunAsync_TextFormat_ReplacesPlaceholdersCorrectly(string textFormat, string expectedPattern)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerTextFormatTest_{Guid.NewGuid():N}");

        try
        {
            // Setup test repository
            Directory.CreateDirectory(tempPath);

            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.name \"Test User\"");

            // Create initial commit
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "test content");
            await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
            await TestUtilities.RunGitCommandAsync(tempPath, "commit -m \"Initial commit\"");

            // Create Processor with logger
            using var outWriter = new StringWriter();
            using var errorWriter = new StringWriter();
            var logger = Logger.Create("ProcessorTest", LogImportance.Normal, outWriter, errorWriter, errorWriter);
            var processor = new Processor(logger);

            // Setup ProcessorContext with Text language for testing TextFormat
            var outputPath = Path.Combine(tempPath, "textformat_output.txt");
            var context = new ProcessorContext
            {
                ProjectDirectory = tempPath,
                Language = "Text",
                OutputPath = outputPath,
                TextFormat = textFormat,
                IsDryRun = false
            };
            
            //-------------------------------------------------------------

            // Execute RunAsync
            var result = await processor.RunAsync(context, CancellationToken.None);
            
            //-------------------------------------------------------------

            // Load output file was created
            Assert.That(File.Exists(outputPath), Is.True, "Output file should be created");
            var outputContent = await TestUtilities.ReadAllTextAsync(outputPath);
            
            //-------------------------------------------------------------

            // Get actual values for placeholder replacement
            var placeholders = new Dictionary<string, string>
            {
                [PlaceholderKeys.VERSION] = result.Version.ToString(),
                [PlaceholderKeys.ASSEMBLY_VERSION] = result.Version.ToString(),
                [PlaceholderKeys.FILE_VERSION] = result.SafeVersion.ToString(),
                [PlaceholderKeys.APPLICATION_VERSION] = result.EpochIntDateVersion,
                [PlaceholderKeys.APPLICATION_DISPLAY_VERSION] = result.ShortVersion,
                [PlaceholderKeys.COMMIT_ID] = result.CommitId,
                [PlaceholderKeys.BRANCH] = result.Branch ?? "main",
                [PlaceholderKeys.TAGS] = string.Join(",", result.Tags),
                [PlaceholderKeys.AUTHOR] = result.Author,
                [PlaceholderKeys.COMMITTER] = result.Committer,
                [PlaceholderKeys.SUBJECT] = result.Subject ?? "",
                [PlaceholderKeys.BODY] = result.Body ?? "",
                [PlaceholderKeys.DATE] = result.Date.ToString("dddd, dd MMMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture).Replace(":", "")
            };

            // Replace placeholders in expected content and normalize both for comparison
            var normalizedExpectedPattern = PlaceholderHelper.ReplacePlaceholders(expectedPattern, placeholders);

            // Normalize output for comparison
            var normalizedOutput = PlaceholderHelper.NormalizeOutputForComparison(outputContent);

            // Verify the output matches the expected pattern
            Assert.That(normalizedOutput, Is.EqualTo(normalizedExpectedPattern), 
                $"TextFormat '{textFormat}' should produce expected output");

            // Verify result is not null
            Assert.That(result, Is.Not.Null, "Result should not be null");
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }

    [TestCase(null, false, "")]
    [TestCase("", false, "")]
    [TestCase("MyNamespace", true, "namespace MyNamespace")]
    [TestCase("Company.Project", true, "namespace Company.Project")]
    [TestCase("Test.Deep.Nested.Namespace", true, "namespace Test.Deep.Nested.Namespace")]
    public async Task RunAsync_Namespace_AppliesCorrectNamespace(string? namespaceValue, bool shouldHaveNamespace, string expectedNamespaceLine)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerNamespaceTest_{Guid.NewGuid():N}");

        try
        {
            // Setup test repository
            Directory.CreateDirectory(tempPath);

            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.name \"Test User\"");

            // Create initial commit
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "test content");
            await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
            await TestUtilities.RunGitCommandAsync(tempPath, "commit -m \"Initial commit\"");

            // Create Processor with logger
            using var outWriter = new StringWriter();
            using var errorWriter = new StringWriter();
            var logger = Logger.Create("ProcessorTest", LogImportance.Normal, outWriter, errorWriter, errorWriter);
            var processor = new Processor(logger);

            // Setup ProcessorContext with C# language for testing Namespace
            var outputPath = Path.Combine(tempPath, "namespace_output.cs");
            var context = new ProcessorContext
            {
                ProjectDirectory = tempPath,
                Language = "C#",
                OutputPath = outputPath,
                Namespace = namespaceValue,
                GenerateStatic = true, // Required for namespace generation
                IsDryRun = false
            };
            
            //-------------------------------------------------------------

            // Execute RunAsync
            var result = await processor.RunAsync(context, CancellationToken.None);
            
            //-------------------------------------------------------------

            // Load output file was created
            Assert.That(File.Exists(outputPath), Is.True, "Output file should be created");
            var outputContent = await TestUtilities.ReadAllTextAsync(outputPath);

            // Verify namespace is applied correctly in ThisAssembly class generation
            if (shouldHaveNamespace)
            {
                Assert.That(outputContent, Does.Contain(expectedNamespaceLine), 
                    $"Namespace declaration '{expectedNamespaceLine}' should be present for namespace value: '{namespaceValue}'");
            }
            else
            {
                // When namespace is null or empty, no user-defined namespace declaration should be present
                // Check that ThisAssembly class appears directly without namespace wrapper
                var lines = outputContent.Split('\n');
                var thisAssemblyLineIndex = Array.FindIndex(lines, line => line.Contains("internal static class ThisAssembly"));
                Assert.That(thisAssemblyLineIndex, Is.GreaterThan(-1), "ThisAssembly class should be found");
                
                // Check that the line before ThisAssembly is not a namespace declaration
                for (int i = Math.Max(0, thisAssemblyLineIndex - 5); i < thisAssemblyLineIndex; i++)
                {
                    var line = lines[i].Trim();
                    if (line.StartsWith("namespace ") && !line.Contains("System.Reflection"))
                    {
                        Assert.Fail($"User-defined namespace declaration '{line}' should not be present for namespace value: '{namespaceValue}'");
                    }
                }
            }

            // Verify that ThisAssembly class exists when GenerateStatic is true
            Assert.That(outputContent, Does.Contain("internal static class ThisAssembly"), 
                "ThisAssembly class should be generated when GenerateStatic is true");

            // Verify result is not null and has expected properties
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result.Version.ToString(), Is.EqualTo("0.0.1"), "Version should be default 0.0.1");
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }

    [TestCase(true, false)]  // IsDryRun = true, file should NOT be created
    [TestCase(false, true)]  // IsDryRun = false, file SHOULD be created
    public async Task RunAsync_IsDryRun_ControlsFileCreation(bool isDryRun, bool shouldFileExist)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerDryRunTest_{Guid.NewGuid():N}");

        try
        {
            // Setup test repository
            Directory.CreateDirectory(tempPath);

            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.name \"Test User\"");

            // Create initial commit
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "test content");
            await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
            await TestUtilities.RunGitCommandAsync(tempPath, "commit -m \"Initial commit\"");

            // Create Processor with logger
            using var outWriter = new StringWriter();
            using var errorWriter = new StringWriter();
            var logger = Logger.Create("ProcessorTest", LogImportance.Normal, outWriter, errorWriter, errorWriter);
            var processor = new Processor(logger);

            // Setup ProcessorContext with Text language for testing IsDryRun
            var outputPath = Path.Combine(tempPath, "dryrun_output.txt");
            var context = new ProcessorContext
            {
                ProjectDirectory = tempPath,
                Language = "Text",
                OutputPath = outputPath,
                TextFormat = "{versionLabel}",
                IsDryRun = isDryRun
            };
            
            //-------------------------------------------------------------

            // Execute RunAsync
            var result = await processor.RunAsync(context, CancellationToken.None);
            
            //-------------------------------------------------------------

            // Verify file creation behavior based on IsDryRun
            Assert.That(File.Exists(outputPath), Is.EqualTo(shouldFileExist), 
                $"File existence should be {shouldFileExist} when IsDryRun is {isDryRun}");

            // Verify that result is always returned regardless of IsDryRun
            Assert.That(result, Is.Not.Null, "Result should not be null even in dry run mode");
            Assert.That(result.Version.ToString(), Is.EqualTo("0.0.1"), "Version should be default 0.0.1");
            Assert.That(result.CommitId, Has.Length.EqualTo(40), "CommitId should be a 40-character hash");
            Assert.That(result.CommitId, Does.Match("^[a-f0-9]{40}$"), "CommitId should be a valid SHA-1 hash");
            Assert.That(result.Author, Is.EqualTo("Test User <test@example.com>"), "Author should match configured user");
            Assert.That(result.Committer, Is.EqualTo("Test User <test@example.com>"), "Committer should match configured user");
            Assert.That(result.Branch, Is.EqualTo("main"), "Branch should be main");

            // If file was created, verify its content
            if (shouldFileExist)
            {
                var content = await TestUtilities.ReadAllTextAsync(outputPath);
                Assert.That(content.Trim(), Is.EqualTo("0.0.1"), "File content should match expected version");
            }
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }

    [TestCase("C#", true)]
    [TestCase("F#", true)]
    [TestCase("VB", true)]
    [TestCase("C++/CLI", true)]
    [TestCase("Text", false)]  // Text language doesn't support namespace
    public async Task RunAsync_IsDryRun_DoesNotCreateFilesForAllLanguages(string language, bool supportsNamespace)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerDryRunLanguageTest_{Guid.NewGuid():N}");

        try
        {
            // Setup test repository
            Directory.CreateDirectory(tempPath);

            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.email \"test@example.com\"");
            await TestUtilities.RunGitCommandAsync(tempPath, "config user.name \"Test User\"");

            // Create initial commit
            var testFile = Path.Combine(tempPath, "test.txt");
            File.WriteAllText(testFile, "test content");
            await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
            await TestUtilities.RunGitCommandAsync(tempPath, "commit -m \"Initial commit\"");

            // Create Processor with logger
            using var outWriter = new StringWriter();
            using var errorWriter = new StringWriter();
            var logger = Logger.Create("ProcessorTest", LogImportance.Normal, outWriter, errorWriter, errorWriter);
            var processor = new Processor(logger);

            // Setup ProcessorContext with IsDryRun = true
            var fileExtension = GetFileExtension(language);
            var outputPath = Path.Combine(tempPath, $"dryrun_output{fileExtension}");
            var context = new ProcessorContext
            {
                ProjectDirectory = tempPath,
                Language = language,
                OutputPath = outputPath,
                Namespace = supportsNamespace ? "Test.Namespace" : null,
                TextFormat = language == "Text" ? "{versionLabel}" : "",
                IsDryRun = true // Dry run mode
            };
            
            //-------------------------------------------------------------

            // Execute RunAsync
            var result = await processor.RunAsync(context, CancellationToken.None);
            
            //-------------------------------------------------------------

            // Verify that no file was created in dry run mode
            Assert.That(File.Exists(outputPath), Is.False, 
                $"No file should be created for language '{language}' in dry run mode");

            // Verify that result is still returned
            Assert.That(result, Is.Not.Null, "Result should not be null even in dry run mode");
            Assert.That(result.Version.ToString(), Is.EqualTo("0.0.1"), "Version should be default 0.0.1");
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }

    /// <summary>
    /// Get appropriate file extension for the language
    /// </summary>
    private static string GetFileExtension(string language) => language switch
    {
        "C#" => ".cs",
        "F#" => ".fs",
        "VB" => ".vb",
        "C++/CLI" => ".cpp",
        "Text" => ".txt",
        _ => ".txt"
    };

    /// <summary>
    /// Test commit history variations
    /// </summary>
    [TestCase("SingleCommit", "Test User", "test@example.com", "Initial commit", "")]
    [TestCase("MultipleCommits", "Test User", "test@example.com", "Latest commit", "")]
    [TestCase("DifferentAuthor", "Jane Developer", "jane@example.com", "Feature implementation", "")]
    [TestCase("DifferentCommitMessage", "Test User", "test@example.com", "Add new feature with detailed description", "")]
    [TestCase("WithCommitBody", "Test User", "test@example.com", "Fix critical bug", "This commit fixes issue #123 by updating the validation logic")]
    public async Task RunAsync_CommitHistory_ReflectsGitState(string testCase, string authorName, string authorEmail, string commitSubject, string commitBody)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerCommitHistory_{testCase}_{Guid.NewGuid():N}");

        try
        {
            // Setup test repository
            Directory.CreateDirectory(tempPath);

            await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
            await TestUtilities.RunGitCommandAsync(tempPath, $"config user.email \"{authorEmail}\"");
            await TestUtilities.RunGitCommandAsync(tempPath, $"config user.name \"{authorName}\"");

            // Create commits based on test case
            if (testCase == "SingleCommit")
            {
                // Single commit
                var testFile = Path.Combine(tempPath, "test.txt");
                File.WriteAllText(testFile, "test content");
                await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
                await TestUtilities.RunGitCommandAsync(tempPath, $"commit -m \"{commitSubject}\"");
            }
            else if (testCase == "MultipleCommits")
            {
                // Multiple commits - first commit
                var testFile1 = Path.Combine(tempPath, "file1.txt");
                File.WriteAllText(testFile1, "first file");
                await TestUtilities.RunGitCommandAsync(tempPath, "add file1.txt");
                await TestUtilities.RunGitCommandAsync(tempPath, "commit -m \"First commit\"");

                // Second commit (latest)
                var testFile2 = Path.Combine(tempPath, "file2.txt");
                File.WriteAllText(testFile2, "second file");
                await TestUtilities.RunGitCommandAsync(tempPath, "add file2.txt");
                await TestUtilities.RunGitCommandAsync(tempPath, $"commit -m \"{commitSubject}\"");
            }
            else if (testCase == "WithCommitBody")
            {
                // Commit with body
                var testFile = Path.Combine(tempPath, "test.txt");
                File.WriteAllText(testFile, "test content");
                await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
                var fullCommitMessage = $"{commitSubject}\n\n{commitBody}";
                await TestUtilities.RunGitCommandAsync(tempPath, $"commit -m \"{fullCommitMessage}\"");
                
                // Extract body from actual Git commit message using the same logic as Processor
                var actualCommitMessage = await TestUtilities.RunGitCommandWithOutputAsync(tempPath, "log --format=\"%B\" -n 1");
                var index = actualCommitMessage.IndexOf("\n\n", StringComparison.InvariantCulture);
                // Use same extraction logic as Processor - preserve trailing newlines
                commitBody = (index >= 0) ? actualCommitMessage.Substring(index + 2) : string.Empty;
            }
            else
            {
                // Default single commit for other test cases
                var testFile = Path.Combine(tempPath, "test.txt");
                File.WriteAllText(testFile, "test content");
                await TestUtilities.RunGitCommandAsync(tempPath, "add test.txt");
                await TestUtilities.RunGitCommandAsync(tempPath, $"commit -m \"{commitSubject}\"");
            }

            // Create Processor with logger
            using var outWriter = new StringWriter();
            using var errorWriter = new StringWriter();
            var logger = Logger.Create("ProcessorTest", LogImportance.Normal, outWriter, errorWriter, errorWriter);
            var processor = new Processor(logger);

            // Setup ProcessorContext
            var outputPath = Path.Combine(tempPath, "commit_history_output.cs");
            var context = new ProcessorContext
            {
                ProjectDirectory = tempPath,
                Language = "C#",
                OutputPath = outputPath,
                IsDryRun = false
            };

            //-------------------------------------------------------------

            // Execute RunAsync
            var result = await processor.RunAsync(context, CancellationToken.None);
            
            //-------------------------------------------------------------

            // Load output file was created
            Assert.That(File.Exists(outputPath), Is.True, "Output file should be created");
            var outputContent = await TestUtilities.ReadAllTextAsync(outputPath);

            // Load expected content from asset file
            var assetPath = Path.Combine("artifacts", "expected_CommitHistory.txt");
            Assert.That(File.Exists(assetPath), Is.True, $"Asset file should exist: {assetPath}");
            var expectedContent = await TestUtilities.ReadAllTextAsync(assetPath);

            // Get actual values for placeholder replacement
            var placeholders = new Dictionary<string, string>
            {
                [PlaceholderKeys.VERSION] = result.Version.ToString(),
                [PlaceholderKeys.ASSEMBLY_VERSION] = result.Version.ToString(),
                [PlaceholderKeys.FILE_VERSION] = result.SafeVersion.ToString(),
                [PlaceholderKeys.APPLICATION_VERSION] = result.EpochIntDateVersion,
                [PlaceholderKeys.APPLICATION_DISPLAY_VERSION] = result.ShortVersion,
                [PlaceholderKeys.COMMIT_ID] = result.CommitId,
                [PlaceholderKeys.BRANCH] = result.Branch ?? "main",
                [PlaceholderKeys.TAGS] = string.Join(",", result.Tags),
                [PlaceholderKeys.AUTHOR] = result.Author,
                [PlaceholderKeys.COMMITTER] = result.Committer,
                [PlaceholderKeys.SUBJECT] = result.Subject ?? "",
                [PlaceholderKeys.BODY] = result.Body ?? "",
                [PlaceholderKeys.DATE] = result.Date.ToString("dddd, dd MMMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture).Replace(":", "")
            };

            // Replace placeholders in expected content
            expectedContent = PlaceholderHelper.ReplacePlaceholders(expectedContent, placeholders);

            // Normalize outputs for comparison
            var normalizedOutput = PlaceholderHelper.NormalizeOutputForComparison(outputContent);
            var normalizedExpected = PlaceholderHelper.NormalizeOutputForComparison(expectedContent);

            // Compare outputs
            Assert.That(normalizedOutput, Is.EqualTo(normalizedExpected), 
                $"Output content should match expected content for test case: {testCase}");
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }
} 