////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
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
    public string ProjectDirectory;
    public string OutputPath;
    public string Language;
    public string Namespace;
    public string TargetFramework;
    public string TargetFrameworkIdentity;
    public string TargetFrameworkVersion;
    public string TargetFrameworkProfile;
    public bool GenerateStatic;
    public string BuildIdentifier;
    public string PropertiesPath;
}

public sealed class Processor
{
    private readonly Logger logger;
    private readonly Dictionary<string, WriterBase> writers;

    public Processor(Logger logger)
    {
        this.logger = logger;
        this.writers = Utilities.GetWriters();
        this.Languages = this.writers.Values.
            Select(writer => writer.Language).
            ToArray();
    }

    public string[] Languages { get; }

    private readonly struct TargetCommit
    {
        public readonly int StartDepth;
        public readonly Commit Commit;

        public TargetCommit(int startDepth, Commit commit)
        {
            this.StartDepth = startDepth;
            this.Commit = commit;
        }

        public override string ToString() =>
            $"StartDepth={this.StartDepth}, {this.Commit}";
    }

    private static async Task<Result> WriteVersionSourceFileAsync(
        Logger logger,
        WriterBase writer,
        ProcessorContext context,
        Branch targetBranch,
        DateTimeOffset generated,
        IEnumerable<Rule> ruleSet,
        IEnumerable<string> importSet,
        CancellationToken ct)
    {
        Debug.Assert(ruleSet != null);
        Debug.Assert(importSet != null);

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

        var versionLabelTask = targetBranch is { } ?
            Analyzer.LookupVersionLabelAsync(targetBranch, ct) :
            Task.FromResult(Version.Default);
        var keyValues =
            (!string.IsNullOrWhiteSpace(context.PropertiesPath) &&
             File.Exists(context.PropertiesPath)) ?
             XDocument.Load(context.PropertiesPath).
             Root.Elements().
             ToDictionary(e => e.Name.LocalName, e => (object)e.Value) :
             new Dictionary<string, object>();

        var versionLabel = await versionLabelTask;

        var shortVersion = versionLabel.ToString(3);

        foreach (var entry in new (string key, object value)[]
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

        if (!string.IsNullOrWhiteSpace(context.OutputPath))
        {
            writer.Write(context, keyValues, generated, ruleSet, importSet);
        }

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
        var writer = writers[context.Language];

        var elementSets = Utilities.GetElementSets(
            Utilities.LoadRuleSets(context.ProjectDirectory).
                Concat(new[] { Utilities.GetDefaultRuleSet() }));

        var elementSet = elementSets[context.Language];
        var importSet = Utilities.AggregateImports(elementSet);
        var ruleSet = Utilities.AggregateRules(elementSet);

        // Traverse git repository between projectDirectory and the root.
        using var repository = await Utilities.OpenRepositoryAsync(
            logger, context.ProjectDirectory);

        try
        {
            return await WriteVersionSourceFileAsync(
                logger,
                writer,
                context,
                repository?.Head,
                DateTimeOffset.Now,
                ruleSet,
                importSet,
                ct);
        }
        finally
        {
            repository?.Dispose();
        }
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
                    File.Move(path, originPath);
                    originExists = true;
                }
                catch
                {
                }
            }

            try
            {
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
