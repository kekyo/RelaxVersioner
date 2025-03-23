////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

#nullable enable

using NamingFormatter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RelaxVersioner.Writers;

internal abstract class SourceCodeWriteProviderBase : WriteProviderBase
{
    private void Write(
        ProcessorContext context,
        Dictionary<string, object?> keyValues,
        DateTimeOffset generated,
        IEnumerable<Rule> ruleSet,
        IEnumerable<string> importSet)
    {
        Debug.Assert(string.IsNullOrWhiteSpace(context.OutputPath) == false);

        Processor.WriteSafeTransacted(
            context.OutputPath,
            stream =>
            {
                var tw = new SourceCodeWriter(new StreamWriter(stream), context);

                this.WriteComment(tw,
                    $"This is auto-generated version information attributes by RelaxVersioner [{ThisAssembly.AssemblyVersion}], Do not edit.");
                this.WriteComment(tw,
                    $"Generated date: {generated:F}");
                tw.WriteLine();

                this.WriteBeforeBody(tw);

                foreach (var importNamespace in importSet)
                {
                    this.WriteImport(tw, importNamespace);
                }
                tw.WriteLine();

                foreach (var rule in ruleSet)
                {
                    var formattedValue = Named.Format(
                        CultureInfo.InvariantCulture,
                        rule.Format,
                        keyValues,
                        key => string.Empty,
                        new(context.BracketStart, context.BracketEnd));
                    if (!string.IsNullOrWhiteSpace(rule.Key))
                    {
                        this.WriteAttributeWithArguments(tw, rule.Name, rule.Key, formattedValue);
                    }
                    else
                    {
                        this.WriteAttributeWithArguments(tw, rule.Name, formattedValue);
                    }
                }
                tw.WriteLine();

                if (context.GenerateStatic)
                {
                    this.WriteBeforeLiteralBody(tw);

                    foreach (var g in ruleSet.GroupBy(rule => rule.Name))
                    {
                        var rules = g.ToArray();

                        if (rules.Length >= 2)
                        {
                            this.WriteBeforeNestedLiteralBody(tw, rules[0].Name);
                        }

                        foreach (var rule in rules)
                        {
                            var formattedValue = Named.Format(
                                CultureInfo.InvariantCulture,
                                rule.Format,
                                keyValues,
                                key => string.Empty,
                                new(context.BracketStart, context.BracketEnd));
                            if (!string.IsNullOrWhiteSpace(rule.Key))
                            {
                                this.WriteLiteralWithArgument(tw, rule.Key, formattedValue);
                            }
                            else
                            {
                                this.WriteLiteralWithArgument(tw, rule.Name, formattedValue);
                            }
                        }

                        if (rules.Length >= 2)
                        {
                            this.WriteAfterNestedLiteralBody(tw);
                        }
                    }

                    this.WriteAfterLiteralBody(tw);
                    tw.WriteLine();
                }

                this.WriteAfterBody(tw);

                tw.Flush();
            });
    }

    public override sealed void Write(
        ProcessorContext context,
        Dictionary<string, object?> keyValues,
        DateTimeOffset generated)
    {
        var elementSets = Utilities.GetElementSets(
            Utilities.LoadRuleSets(context.ProjectDirectory).
                Concat(new[] { Utilities.GetDefaultRuleSet() }));

        var elementSet = elementSets[context.Language];
        var importSet = Utilities.AggregateImports(elementSet);
        var ruleSet = Utilities.AggregateRules(elementSet);

        if (context.IsDryRun ||
            string.IsNullOrWhiteSpace(context.OutputPath))
        {
            return;
        }

        this.Write(context, keyValues, generated, ruleSet, importSet);
    }

    protected static bool IsRequiredSelfHostingMetadataAttribute(ProcessorContext context) =>
        (context.TargetFrameworkIdentity == ".NETFramework") &&
        context.TargetFrameworkVersion is { } tfv &&
        tfv.StartsWith("v") &&
        tfv.Substring(1) is { } vs &&
        Version.TryParse(vs, out var version) &&
        ((version.Major < 4) ||
         (version.Major == 4) && (version.Minor == 0));

    protected virtual void WriteComment(SourceCodeWriter tw, string format, params object?[] args) =>
        tw.WriteLine("// " + format, args);

    protected virtual void WriteBeforeBody(SourceCodeWriter tw)
    {
    }

    protected abstract void WriteAttribute(SourceCodeWriter tw, string name, string args);
    protected abstract void WriteLiteral(SourceCodeWriter tw, string name, string value);

    protected virtual string NormalizeControlChars(string str) =>
        Utilities.NormalizeControlCharsForCLike(str);

    protected virtual string GetArgumentString(string argumentValue) =>
        $"\"{NormalizeControlChars(argumentValue.Trim(' ', '\t', '\r', '\n', '\0'))}\"";

    protected abstract void WriteBeforeLiteralBody(SourceCodeWriter tw);
    protected abstract void WriteBeforeNestedLiteralBody(SourceCodeWriter tw, string name);
    protected abstract void WriteAfterNestedLiteralBody(SourceCodeWriter tw);
    protected abstract void WriteAfterLiteralBody(SourceCodeWriter tw);

    private void WriteAttributeWithArguments(SourceCodeWriter tw, string name, params object?[] args) =>
        this.WriteAttribute(
            tw,
            Utilities.MakeSaferChars(name, "_"),
            string.Join(",", args.Select(arg => this.GetArgumentString(arg?.ToString() ?? string.Empty))));

    private void WriteLiteralWithArgument(SourceCodeWriter tw, string name, object? arg) =>
        this.WriteLiteral(
            tw,
            Utilities.MakeSaferChars(name, "_"),
            this.GetArgumentString(arg?.ToString() ?? string.Empty));

    protected virtual void WriteImport(SourceCodeWriter tw, string namespaceName)
    {
    }

    protected virtual void WriteAfterBody(SourceCodeWriter tw)
    {
    }
}
