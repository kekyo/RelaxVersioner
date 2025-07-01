////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RelaxVersioner;

/// <summary>
/// Constant definitions for placeholder keys
/// </summary>
public static class PlaceholderKeys
{
    // Core version placeholders
    public const string RELAXVERSIONER_VERSION = "RELAXVERSIONER_VERSION_PLACEHOLDER";
    public const string VERSION = "VERSION_PLACEHOLDER";
    public const string ASSEMBLY_VERSION = "ASSEMBLY_VERSION_PLACEHOLDER";
    public const string ASSEMBLY_FILE_VERSION = "ASSEMBLY_FILE_VERSION_PLACEHOLDER";
    public const string ASSEMBLY_INFORMATIONAL_VERSION = "ASSEMBLY_INFORMATIONAL_VERSION_PLACEHOLDER";
    public const string ASSEMBLY_CONFIGURATION = "ASSEMBLY_CONFIGURATION_PLACEHOLDER";
    public const string APPLICATION_DISPLAY_VERSION = "APPLICATION_DISPLAY_VERSION_PLACEHOLDER";
    public const string FILE_VERSION = "FILE_VERSION_PLACEHOLDER";
    public const string COMMIT_ID = "COMMIT_ID_PLACEHOLDER";
    public const string BRANCH = "BRANCH_PLACEHOLDER";
    public const string TAGS = "TAGS_PLACEHOLDER";
    public const string AUTHOR = "AUTHOR_PLACEHOLDER";
    public const string COMMITTER = "COMMITTER_PLACEHOLDER";
    public const string SUBJECT = "SUBJECT_PLACEHOLDER";
    public const string BODY = "BODY_PLACEHOLDER";
    public const string DATE = "DATE_PLACEHOLDER";
    public const string APPLICATION_VERSION = "APPLICATION_VERSION_PLACEHOLDER";
    public const string DATE_METADATA = "DATE_METADATA_PLACEHOLDER";
    public const string BRANCH_METADATA = "BRANCH_METADATA_PLACEHOLDER";
    public const string TAGS_METADATA = "TAGS_METADATA_PLACEHOLDER";
    public const string AUTHOR_METADATA = "AUTHOR_METADATA_PLACEHOLDER";
    public const string COMMITTER_METADATA = "COMMITTER_METADATA_PLACEHOLDER";
    public const string SUBJECT_METADATA = "SUBJECT_METADATA_PLACEHOLDER";
    public const string BODY_METADATA = "BODY_METADATA_PLACEHOLDER";
    public const string BUILD_METADATA = "BUILD_METADATA_PLACEHOLDER";
    public const string GENERATED_METADATA = "GENERATED_METADATA_PLACEHOLDER";
    public const string TARGET_FRAMEWORK_MONIKER_METADATA = "TARGET_FRAMEWORK_MONIKER_METADATA_PLACEHOLDER";
    public const string APPLICATION_DISPLAY_VERSION_METADATA = "APPLICATION_DISPLAY_VERSION_METADATA_PLACEHOLDER";
    public const string APPLICATION_VERSION_METADATA = "APPLICATION_VERSION_METADATA_PLACEHOLDER";
    public const string ASSEMBLY_NAME_METADATA = "ASSEMBLY_NAME_METADATA_PLACEHOLDER";
    public const string ROOT_NAMESPACE_METADATA = "ROOT_NAMESPACE_METADATA_PLACEHOLDER";
    public const string PLATFORM_TARGET_METADATA = "PLATFORM_TARGET_METADATA_PLACEHOLDER";
    public const string PLATFORM_METADATA = "PLATFORM_METADATA_PLACEHOLDER";
    public const string RUNTIME_IDENTIFIER_METADATA = "RUNTIME_IDENTIFIER_METADATA_PLACEHOLDER";
    public const string BUILD_ON_METADATA = "BUILD_ON_METADATA_PLACEHOLDER";
    public const string SDK_VERSION_METADATA = "SDK_VERSION_METADATA_PLACEHOLDER";
    public const string GENERATED_DATE_COMMENT = "GENERATED_DATE_COMMENT_PLACEHOLDER";
}

/// <summary>
/// Helper class for placeholder replacement
/// </summary>
public static class PlaceholderHelper
{
    // Default value definitions
    private static readonly Dictionary<string, string> DefaultValues = new()
    {
        [PlaceholderKeys.RELAXVERSIONER_VERSION] = $"[{typeof(Processor).Assembly.GetName().Version!.ToString(3)}]",
        [PlaceholderKeys.VERSION] = "0.0.1",
        [PlaceholderKeys.ASSEMBLY_VERSION] = "0.0.1", 
        [PlaceholderKeys.APPLICATION_DISPLAY_VERSION] = "0.0.1",
        [PlaceholderKeys.BRANCH] = "main",
        [PlaceholderKeys.TAGS] = "",
        [PlaceholderKeys.AUTHOR] = "Test User <test@example.com>",
        [PlaceholderKeys.COMMITTER] = "Test User <test@example.com>",
        [PlaceholderKeys.SUBJECT] = "",
        [PlaceholderKeys.BODY] = "",
    };

    /// <summary>
    /// Replace placeholders with key-value pairs
    /// </summary>
    /// <param name="content">Content to be replaced</param>
    /// <param name="customValues">Custom values (uses only default values if null)</param>
    /// <returns>Content after replacement</returns>
    public static string ReplacePlaceholders(
        string content, Dictionary<string, string>? customValues = null)
    {
        var values = new Dictionary<string, string>(DefaultValues);

        // Override with custom values
        if (customValues != null)
        {
            foreach (var kvp in customValues)
            {
                // (Normalize strategy same as SourceCodeWriteProviderBase)
                var value = kvp.Value is { } v ?
                    Utilities.NormalizeControlCharsForCLike(Utilities.TrimUnusableCharacters(v)) :
                    string.Empty;

                values[kvp.Key] = value;
            }
        }

        // Replace placeholders
        var sb = new StringBuilder(content);
        foreach (var kvp in values)
        {
            // (Normalize strategy same as SourceCodeWriteProviderBase)
            var value = kvp.Value is { } v ?
                Utilities.NormalizeControlCharsForCLike(Utilities.TrimUnusableCharacters(v)) :
                string.Empty;

            sb.Replace($"[{kvp.Key}]", value);
        }
        content = sb.ToString();

        // Remove date comment lines (for test comparison)
        //content = Regex.Replace(
        //    content, 
        //    @"// Generated date: \[DATE_PLACEHOLDER\]\r?\n", 
        //    "", 
        //    RegexOptions.Multiline);
        //content = Regex.Replace(
        //    content, 
        //    @"' Generated date: \[DATE_PLACEHOLDER\]\r?\n", 
        //    "", 
        //    RegexOptions.Multiline);

        return content;
    }

    /// <summary>
    /// Normalize output for comparison by replacing dynamic values with placeholders
    /// </summary>
    /// <param name="output">Content to normalize</param>
    /// <returns>Normalized content with placeholders</returns>
    public static string NormalizeOutputForComparison(string output)
    {
        // Handle multiple date format patterns that might appear in actual output
        var normalizedOutput = output;
        
        // Pattern 1: Date metadata format "{commitDate:F} {commitDate.Offset:hhmm}"
        // Example: "Tuesday, July 1, 2025 10:34:32 PM +0900"
        normalizedOutput = Regex.Replace(normalizedOutput, 
            @"""Date"",""[^""]*""", 
            $@"""Date"",""{PlaceholderKeys.DATE_METADATA}""");
            
        // Pattern 2: Generated metadata format "{generated:F}"  
        // Example: "Tuesday, July 1, 2025 10:34:32 PM"
        normalizedOutput = Regex.Replace(normalizedOutput,
            @"""Generated"",""[^""]*""",
            $@"""Generated"",""{PlaceholderKeys.GENERATED_METADATA}""");
            
        // Pattern 3: Generated date in comments
        // Example: "Generated date: Tuesday, July 1, 2025 10:34:32 PM"
        normalizedOutput = Regex.Replace(normalizedOutput,
            @"Generated date: [^\r\n]*",
            $"Generated date: {PlaceholderKeys.GENERATED_DATE_COMMENT}");

        // Pattern 4: Various timestamp formats in metadata
        normalizedOutput = Regex.Replace(normalizedOutput,
            @"""ApplicationVersion"",""[^""]*""",
            $@"""ApplicationVersion"",""{PlaceholderKeys.APPLICATION_VERSION_METADATA}""");
            
        // Replace safeVersion with placeholder (YYYY.M.D.NUMBER format)
        normalizedOutput = Regex.Replace(
            normalizedOutput,
            @"\d{4}\.\d+\.\d+\.\d+", 
            "[SAFE_VERSION_PLACEHOLDER]");

        return normalizedOutput;
    }
} 