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
using GitReader.IO;
using GitReader.Primitive;

using RelaxVersioner.Writers;

namespace RelaxVersioner;

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
        PrimitiveRepository? repository,
        PrimitiveReference? targetBranch,
        DateTimeOffset generated,
        CancellationToken ct)
    {
        var commit = (repository is { } r1 && targetBranch is { } tb) ?
            await r1.GetCommitAsync(tb, ct) : null;

        var commitId = commit?.Hash.ToString() ??
            "unknown";

        static string FormatSignature(Signature? sig) => sig is { } s ?
            (s.MailAddress is { } ma ? $"{s.Name} <{ma}>" : s.Name) :
            "(Unknown)";

        var author = FormatSignature(commit?.Author);
        var committer = FormatSignature(commit?.Committer);
        var commitDate = commit?.Committer.Date ?? generated;

        var branches = (repository is { } r2 && commit is { } c1) ?
            (await r2.GetBranchHeadReferencesAsync(ct)).
                Where(b => b.Target.Equals(c1.Hash)).
                Select(b => b.Name).
                ToArray() :
            [];
        var branchesString = string.Join(",", branches);

        var tags = (repository is { } r3 && commit is { } c2) ?
            (await LooseConcurrentScope.Default.WhenAll(
                (await r3.GetTagReferencesAsync(ct)).
                Select(t => r3.GetTagAsync(t, ct)))).
            Where(t => t.Hash.Equals(c2.Hash)).
            Select(b => b.Name).
            ToArray() :
            [];
        var tagsString = string.Join(",", tags);

        var safeVersion = Utilities.GetSafeVersionFromDate(commitDate);
        var intDateVersion = Utilities.GetIntDateVersionFromDate(commitDate);
        var epochIntDateVersion = Utilities.GetEpochIntDateVersionFromDate(commitDate);

        var versionLabelTask = repository is { } r4 ?
            Analyzer.LookupVersionLabelAsync(r4, context.CheckWorkingDirectoryStatus, ct) :
            Task.FromResult(Version.Default);
        var keyValues =
            (!string.IsNullOrWhiteSpace(context.PropertiesPath) && File.Exists(context.PropertiesPath)) ?
                 XDocument.Load(context.PropertiesPath!).
                 Root!.Elements().
                 ToDictionary(e => e.Name.LocalName, e => (object?)e.Value) :
                 new Dictionary<string, object?>();

        var versionLabel = await versionLabelTask;

        var shortVersion = versionLabel.ToString(3);

        // Extract subject and body before writing to ensure they're available in keyValues
        var (subject, body) = commit?.CrackMessage() ?? new(null!, null!);

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
            ("subject", subject),
            ("body", body),
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
            subject,
            body);
    }

    public async Task<Result> RunAsync(
        ProcessorContext context, CancellationToken ct)
    {
        var writeProvider = writeProviders[context.Language];

        // Traverse git repository between projectDirectory and the root.
        using var repository = await Utilities.OpenRepositoryAsync(
            logger, context.ProjectDirectory);

        var targetBranch = repository is { } r ?
            await r.GetCurrentHeadReferenceAsync(ct) : null;
        
        return await WriteVersionSourceFileAsync(
            logger,
            writeProvider,
            context,
            repository,
            targetBranch,
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
