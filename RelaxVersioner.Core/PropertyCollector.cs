////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using NamingFormatter;
using Newtonsoft.Json.Linq;

namespace RelaxVersioner;

internal static class PropertyCollector
{
    private const string JsonOutputSentinelKey = "_RelaxVersioner_PropertyCollectionSentinel_527247488f3645d8a4195fd9059eb403";

    private static readonly HashSet<string> builtInKeys = new(StringComparer.Ordinal)
    {
        "generated",
        "branch",
        "branches",
        "tags",
        "commit",
        "author",
        "committer",
        "commitId",
        "commitDate",
        "versionLabel",
        "shortVersion",
        "safeVersion",
        "intDateVersion",
        "epochIntDateVersion",
        "buildIdentifier",
        "namespace",
        "tfm",
        "tfid",
        "tfv",
        "tfp",
        "subject",
        "body",
    };

    public static async Task<Dictionary<string, object?>> LoadAsync(
        Logger logger,
        ProcessorContext context,
        CancellationToken ct)
    {
        var requestedPropertyNames = CollectRequestedMsBuildPropertyNames(context);

        logger.Message(
            LogImportance.Low,
            "Property collection mode={0}, RequestedProperties={1}",
            context.PropertyCollectionMode,
            requestedPropertyNames.Length >= 1 ?
                string.Join(",", requestedPropertyNames) :
                "(none)");

        var legacyProperties =
            (context.PropertyCollectionMode == PropertyCollectionMode.Legacy) ||
            (context.PropertyCollectionMode == PropertyCollectionMode.Compare) ?
            LoadLegacyProperties(context.PropertiesPath) :
            new Dictionary<string, string>(StringComparer.Ordinal);

        if (context.PropertyCollectionMode == PropertyCollectionMode.Legacy)
        {
            return ToObjectDictionary(legacyProperties);
        }

        if (requestedPropertyNames.Length <= 0)
        {
            return context.PropertyCollectionMode == PropertyCollectionMode.Compare ?
                ToObjectDictionary(legacyProperties) :
                new Dictionary<string, object?>(StringComparer.Ordinal);
        }

        try
        {
            var selectiveProperties = await LoadSelectivePropertiesAsync(
                logger,
                context,
                requestedPropertyNames,
                ct);

            if (context.PropertyCollectionMode == PropertyCollectionMode.Compare)
            {
                WarnOnDifferences(logger, legacyProperties, selectiveProperties, requestedPropertyNames);
                return ToObjectDictionary(legacyProperties);
            }

            return ToObjectDictionary(selectiveProperties);
        }
        catch (Exception ex)
        {
            if (context.PropertyCollectionMode == PropertyCollectionMode.Compare)
            {
                logger.Warning(ex, "Selective property collection preview failed and will fall back to legacy mode.");
                return ToObjectDictionary(legacyProperties);
            }

            throw;
        }
    }

    internal static string[] CollectRequestedMsBuildPropertyNames(ProcessorContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var options = new FormatOptions(
            context.BracketStart ?? "{",
            context.BracketEnd ?? "}");
        var requestedPropertyNames = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var format in EnumerateFormats(context))
        {
            foreach (var reference in Named.GetKeyReferences(format, options))
            {
                var rootKey = reference.RootKey;
                if (string.IsNullOrWhiteSpace(rootKey) ||
                    builtInKeys.Contains(rootKey) ||
                    !seen.Add(rootKey))
                {
                    continue;
                }

                requestedPropertyNames.Add(rootKey);
            }
        }

        return requestedPropertyNames.ToArray();
    }

    private static IEnumerable<string> EnumerateFormats(ProcessorContext context)
    {
        switch (context.Language)
        {
            case "Text":
            case "NPM":
                if (!string.IsNullOrWhiteSpace(context.TextFormat))
                {
                    yield return context.TextFormat;
                }
                yield break;

            case "Replace":
                if (context.ReplaceInputText is { } bufferedInput)
                {
                    yield return bufferedInput;
                    yield break;
                }

                if (context.ReplaceInputPath is { } replaceInputPath &&
                    File.Exists(replaceInputPath))
                {
                    yield return File.ReadAllText(replaceInputPath, Utilities.UTF8);
                }
                yield break;

            default:
                var elementSets = Utilities.GetElementSets(
                    Utilities.LoadRuleSets(context.ProjectDirectory).
                        Concat(new[] { Utilities.GetDefaultRuleSet() }));

                if (elementSets.TryGetValue(context.Language, out var elementSet))
                {
                    foreach (var rule in Utilities.AggregateRules(elementSet))
                    {
                        if (!string.IsNullOrWhiteSpace(rule.Format))
                        {
                            yield return rule.Format;
                        }
                    }
                }
                yield break;
        }
    }

    private static Dictionary<string, string> LoadLegacyProperties(string? propertiesPath)
    {
        if (string.IsNullOrWhiteSpace(propertiesPath) ||
            !File.Exists(propertiesPath))
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        return XDocument.Load(propertiesPath).
            Root!.
            Elements().
            ToDictionary(
                e => e.Name.LocalName,
                e => e.Value,
                StringComparer.Ordinal);
    }

    private static async Task<Dictionary<string, string>> LoadSelectivePropertiesAsync(
        Logger logger,
        ProcessorContext context,
        IReadOnlyCollection<string> requestedPropertyNames,
        CancellationToken ct)
    {
        var projectPath = Utilities.ResolveProjectPath(
            context.ProjectPath ?? context.ProjectDirectory);
        if (string.IsNullOrWhiteSpace(projectPath))
        {
            throw new InvalidOperationException(
                $"MSBuild project path could not be resolved for selective property collection: {context.ProjectPath ?? context.ProjectDirectory}");
        }
        var resolvedProjectPath = projectPath!;

        var invocation = Utilities.ResolveMsBuildInvocation(
            context.MsBuildRuntimeType,
            context.MsBuildBinPath);

        var propertyNames = requestedPropertyNames.
            Concat(new[] { JsonOutputSentinelKey }).
            ToArray();
        var globalProperties = LoadGlobalProperties(context.GlobalPropertiesPath);

        var responseFilePath = Path.Combine(
            Path.GetTempPath(),
            $"RelaxVersioner_{Guid.NewGuid():N}.rsp");

        try
        {
            var responseLines = new List<string>
            {
                "-nologo",
                "-verbosity:quiet",
                $"-getProperty:{string.Join(",", propertyNames)}",
            };
            if (!string.IsNullOrWhiteSpace(context.PropertyCollectionTargetName))
            {
                responseLines.Add(
                    $"-target:{EscapeResponseFileArgument(context.PropertyCollectionTargetName!)}");
            }

            foreach (var entry in globalProperties)
            {
                responseLines.Add(
                    $"-property:{entry.Key}={EscapeMsBuildPropertyValue(entry.Value)}");
            }

            File.WriteAllLines(
                responseFilePath,
                responseLines,
                Utilities.UTF8);

            logger.Message(
                LogImportance.Low,
                "Collecting selective properties: Command={0} {1}",
                invocation.FileName,
                invocation.ArgumentsPrefix);

            var arguments =
                $"{invocation.ArgumentsPrefix}{QuoteArgument(resolvedProjectPath)} @{QuoteArgument(responseFilePath)}";
            var (exitCode, standardOutput, standardError) = await Utilities.RunProcessAsync(
                invocation.FileName,
                Utilities.GetDirectoryNameWithoutTrailingSeparator(
                    Path.GetDirectoryName(resolvedProjectPath) ?? Environment.CurrentDirectory),
                arguments,
                ct);

            if (exitCode != 0)
            {
                throw new InvalidOperationException(
                    $"MSBuild property collection failed: ExitCode={exitCode}, Error={standardError}");
            }

            return ParseJsonProperties(standardOutput);
        }
        finally
        {
            try
            {
                File.Delete(responseFilePath);
            }
            catch
            {
            }
        }
    }

    private static IReadOnlyList<KeyValuePair<string, string>> LoadGlobalProperties(string? globalPropertiesPath)
    {
        if (string.IsNullOrWhiteSpace(globalPropertiesPath) ||
            !File.Exists(globalPropertiesPath))
        {
            return Array.Empty<KeyValuePair<string, string>>();
        }

        return XDocument.Load(globalPropertiesPath).
            Root?.
            Elements("Property").
            Select(property => new KeyValuePair<string, string>(
                property.Attribute("Name")?.Value ?? string.Empty,
                property.Value ?? string.Empty)).
            Where(entry => !string.IsNullOrWhiteSpace(entry.Key)).
            ToArray() ??
            Array.Empty<KeyValuePair<string, string>>();
    }

    private static Dictionary<string, string> ParseJsonProperties(string standardOutput)
    {
        var root = JObject.Parse(standardOutput);
        var properties = root["Properties"] as JObject ??
            throw new InvalidOperationException(
                "MSBuild property collection did not produce a Properties object.");

        return properties.Properties().
            Where(property => property.Name != JsonOutputSentinelKey).
            ToDictionary(
                property => property.Name,
                property => property.Value.Type == JTokenType.Null ?
                    string.Empty :
                    property.Value.ToString(),
                StringComparer.Ordinal);
    }

    private static void WarnOnDifferences(
        Logger logger,
        IReadOnlyDictionary<string, string> legacyProperties,
        IReadOnlyDictionary<string, string> selectiveProperties,
        IEnumerable<string> requestedPropertyNames)
    {
        foreach (var key in requestedPropertyNames)
        {
            var legacyValue = legacyProperties.TryGetValue(key, out var legacy) ?
                legacy ?? string.Empty :
                string.Empty;
            var selectiveValue = selectiveProperties.TryGetValue(key, out var selective) ?
                selective ?? string.Empty :
                string.Empty;

            if (string.Equals(legacyValue, selectiveValue, StringComparison.Ordinal))
            {
                continue;
            }

            logger.Warning(
                "Selective property collection preview mismatch: Key={0}, Legacy={1}, Selective={2}",
                key,
                DescribeValue(legacyValue),
                DescribeValue(selectiveValue));
        }
    }

    private static Dictionary<string, object?> ToObjectDictionary(
        IReadOnlyDictionary<string, string> properties)
    {
        var dictionary = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var entry in properties)
        {
            dictionary[entry.Key] = entry.Value;
        }
        return dictionary;
    }

    private static string QuoteArgument(string argument) =>
        $"\"{argument.Replace("\"", "\\\"")}\"";

    private static string EscapeResponseFileArgument(string value) =>
        value.IndexOfAny(new[] { ' ', '\t', ';', '"' }) >= 0 ?
            QuoteArgument(value) :
            value;

    private static string EscapeMsBuildPropertyValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value.IndexOfAny(new[] { ' ', '\t', ';', '"' }) >= 0 ?
            QuoteArgument(value) :
            value;
    }

    private static string DescribeValue(string value) =>
        string.IsNullOrEmpty(value) ?
            "\"\"" :
            $"\"{value.Replace("\r", "\\r").Replace("\n", "\\n")}\"";
}
