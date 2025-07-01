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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using GitReader;
using GitReader.Structures;

using RelaxVersioner.Writers;

namespace RelaxVersioner;

public sealed class ProcessorContext
{
    public string ProjectDirectory = null!;
    public string OutputPath = null!;
    public string Language = null!;
    public string? Namespace;
    public string? TargetFramework;
    public string? TargetFrameworkIdentity;
    public string? TargetFrameworkVersion;
    public string? TargetFrameworkProfile;
    public bool GenerateStatic;
    public string? BuildIdentifier;
    public string? PropertiesPath;
    public string TextFormat = "{versionLabel}";
    public string? ReplaceInputPath;
    public string BracketStart = "{";
    public string BracketEnd = "}";
    public bool IsDryRun;
    public bool IsQuietOnStandardOutput;
    public bool CheckWorkingDirectoryStatus;
    public string[]? NpmPrefixes;
}

public sealed class Processor
{
    private readonly Logger logger;
    private readonly Dictionary<string, WriteProviderBase> writeProviders;

    public Processor(Logger logger)
    {
        this.logger = logger;
        this.writeProviders = Utilities.GetWriteProviders();
        this.Languages = this.writeProviders.Values.
            Select(writer => writer.Language).
            ToArray();
    }

    public string[] Languages { get; }

    private static async Task<Result> WriteVersionSourceFileAsync(
        Logger logger,
        WriteProviderBase writeProvider,
        ProcessorContext context,
        StructuredRepository? repository,
        Branch? targetBranch,
        DateTimeOffset generated,
        CancellationToken ct)
    {
        var commit = targetBranch != null ?
            await targetBranch.GetHeadCommitAsync(ct) :
            null;

        var commitId = commit?.Hash.ToString() ??
            "unknown";

        static string FormatSignature(Signature? sig) => sig is { } s ?
            (s.MailAddress is { } ma ? $"{s.Name} <{ma}>" : s.Name) :
            "(Unknown)";

        var author = FormatSignature(commit?.Author);
        var committer = FormatSignature(commit?.Committer);
        var commitDate = commit?.Committer.Date ?? generated;

        var branches = commit?.Branches.
            Select(b => b.Name).
            ToArray() ?? Array.Empty<string>();
        var branchesString = string.Join(",", branches);

        var tags = commit?.Tags.
            Select(b => b.Name).
            ToArray() ?? Array.Empty<string>();
        var tagsString = string.Join(",", tags);

        var safeVersion = Utilities.GetSafeVersionFromDate(commitDate);
        var intDateVersion = Utilities.GetIntDateVersionFromDate(commitDate);
        var epochIntDateVersion = Utilities.GetEpochIntDateVersionFromDate(commitDate);

        var versionLabelTask = repository is { } ?
            Analyzer.LookupVersionLabelAsync(repository, context.CheckWorkingDirectoryStatus, ct) :
            Task.FromResult(Version.Default);
        var keyValues =
            (!string.IsNullOrWhiteSpace(context.PropertiesPath) &&
             File.Exists(context.PropertiesPath)) ?
             XDocument.Load(context.PropertiesPath).
             Root!.Elements().
             ToDictionary(e => e.Name.LocalName, e => (object?)e.Value) :
             new Dictionary<string, object?>();

        var versionLabel = await versionLabelTask;

        var shortVersion = versionLabel.ToString(3);

        foreach (var entry in new (string key, object? value)[]
        {
            ("generated", generated),
            ("branch", targetBranch),
            ("branches", branchesString),
            ("tags", tagsString),
            ("commit", commit),
            ("author", author),
            ("committer", committer),
            ("commitId", commitId),
            ("commitDate", commitDate),
            ("versionLabel", versionLabel),
            ("shortVersion", shortVersion),
            ("safeVersion", safeVersion),
            ("intDateVersion", intDateVersion),
            ("epochIntDateVersion", epochIntDateVersion),
            ("buildIdentifier", context.BuildIdentifier),
            ("namespace", context.Namespace),
            ("tfm", context.TargetFramework),
            ("tfid", context.TargetFrameworkIdentity),
            ("tfv", context.TargetFrameworkVersion),
            ("tfp", context.TargetFrameworkProfile),
        })
        {
            logger.Message(LogImportance.Low, "Values: {0}={1}", entry.key, entry.value);
            keyValues[entry.key] = entry.value;
        }

        writeProvider.Write(context, keyValues, generated);

        return new Result(
            versionLabel,
            shortVersion,
            safeVersion,
            intDateVersion,
            epochIntDateVersion,
            commitId,
            targetBranch?.Name,
            tags,
            commitDate,
            author,
            committer,
            commit?.Subject,
            commit?.Body);
    }

    public async Task<Result> RunAsync(
        ProcessorContext context, CancellationToken ct)
    {
        var writeProvider = writeProviders[context.Language];

        // Traverse git repository between projectDirectory and the root.
        using var repository = await Utilities.OpenRepositoryAsync(
            logger, context.ProjectDirectory);

        return await WriteVersionSourceFileAsync(
            logger,
            writeProvider,
            context,
            repository,
            repository?.Head,
            DateTimeOffset.Now,
            ct);
    }

    public static void WriteSafeTransacted(
        string path, Action<Stream> action)
    {
        var directoryPath = Utilities.GetDirectoryPath(path);
        if (!Directory.Exists(directoryPath))
        {
            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch
            {
            }
        }

        var temporaryPath = Path.Combine(
            directoryPath, Path.GetTempFileName());
        try
        {
            using (var stream = new FileStream(
                temporaryPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                action(stream);
                stream.Flush();
            }

            var originExists = false;
            var originPath = Path.Combine(
                directoryPath, Path.GetTempFileName());
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(originPath);
                    File.Move(path, originPath);
                    originExists = true;
                }
                catch
                {
                }
            }

            try
            {
                File.Delete(path);
                File.Move(temporaryPath, path);
            }
            catch
            {
                if (originExists)
                {
                    try
                    {
                        File.Move(originPath, path);
                        originExists = false;
                    }
                    catch
                    {
                    }
                }
                throw;
            }

            if (originExists)
            {
                originExists = false;
                try
                {
                    File.Delete(originPath);
                }
                catch
                {
                }
            }
        }
        catch
        {
            File.Delete(temporaryPath);
        }
    }
}
