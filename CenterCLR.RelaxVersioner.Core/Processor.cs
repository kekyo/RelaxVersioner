////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
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
    private static readonly Branch[] emptyBranches = Array.Empty<Branch>();
    private static readonly Tag[] emptyTags = Array.Empty<Tag>();

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
        IEnumerable<string> importSet)
    {
        Debug.Assert(ruleSet != null);
        Debug.Assert(importSet != null);

        var commit = targetBranch?.Head;

        var commitId = commit?.Hash.ToString() ?? string.Empty;
        var author = commit?.Author ?? Signature.Create("(Unknown)", generated);
        var committer = commit?.Committer ?? Signature.Create("(Unknown)", generated);

        var branches = commit?.Branches.
            Select(b => b.Name).
            ToArray() ?? Array.Empty<string>();
        var branchesString = string.Join(",", branches);

        var tags = commit.Tags.
            Select(b => b.Name).
            ToArray() ?? Array.Empty<string>();
        var tagsString = string.Join(",", tags);

        var safeVersion = Utilities.GetSafeVersionFromDate(committer.Date);
        var intDateVersion = Utilities.GetIntDateVersionFromDate(committer.Date);
        var epochIntDateVersion = Utilities.GetEpochIntDateVersionFromDate(committer.Date);

        var versionLabelTask = targetBranch is { } ?
            Analyzer.LookupVersionLabelAsync(targetBranch) :
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
            ("branch", targetBranch.Name),
            ("branches", branchesString),
            ("tags", tagsString),
            ("commit", commit),
            ("author", author),
            ("committer", committer),
            ("commitId", commitId),
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
            commitId.ToString(),
            targetBranch.Name,
            tags,
            committer.Date,
            author.ToString(),
            committer.ToString(),
            commit.Message);
    }

    public async Task<Result> RunAsync(ProcessorContext context)
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
                repository.GetHead(),
                DateTimeOffset.Now,
                ruleSet,
                importSet);
        }
        finally
        {
            repository?.Dispose();
        }
    }
}
