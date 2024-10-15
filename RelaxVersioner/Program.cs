////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Mono.Options;

namespace RelaxVersioner;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var relaxVersionerVersion = ThisAssembly.AssemblyVersion;
        var logger = Logger.Create($"RelaxVersioner [{relaxVersionerVersion}]", LogImportance.Normal, Console.Out, Console.Error, Console.Error);

        try
        {
            var processor = new Processor(logger);
            var languages = string.Join("|", processor.Languages);

            var context = new ProcessorContext
            {
                Language = "Text",
                GenerateStatic = true,
                TextFormat = "{versionLabel}",
                NpmPrefixes = Array.Empty<string>(),
            };

            string? resultPath = null;
            var launchDebugger = false;
            var help = false;

            var options = new OptionSet
            {
                { "language=", $"target language [{languages}]", v => context.Language = v },
                { "namespace=", "applying namespace", v => context.Namespace = v },
                { "tfm=", "target framework moniker definition (TargetFramework)", v => context.TargetFramework = v },
                { "tfid=", "target framework identity definition (TargetFrameworkIdentifier)", v => context.TargetFrameworkIdentity = v },
                { "tfv=", "target framework version definition (TargetFrameworkVersion)", v => context.TargetFrameworkVersion = v },
                { "tfp=", "target framework profile definition (TargetFrameworkProfile)", v => context.TargetFrameworkProfile = v },
                { "genStatic=", $"generate static informations", v => context.GenerateStatic = bool.TryParse(v, out var genStatic) ? genStatic : true },
                { "buildIdentifier=", $"build identifier", v => context.BuildIdentifier = v },
                { "propertiesPath=", $"properties file", v => context.PropertiesPath = v },
                { "o|outputPath=", $"output source file", v => context.OutputPath = v },
                { "resultPath=", $"output result via xml file", v => resultPath = v },
                { "f|format=", $"set text format", v => context.TextFormat = v },
                { "r|replace", "replace standard input", _ =>
                    {
                        context.ReplaceInputPath = null;
                        context.Language = "Replace";
                    }
                },
                { "i|replaceInputPath=", "replace input source file", v =>
                    {
                        context.ReplaceInputPath = v;
                        context.Language = "Replace";
                    }
                },
                { "bracket=", "enable custom bracket", v =>
                    {
                        if (v.Split(',') is { Length: 2 } splitted)
                        {
                            context.BracketStart = splitted[0];
                            context.BracketEnd = splitted[1];
                        }
                    }
                },
                { "n|npm", "replace NPM package.json", v =>
                    {
                        context.Language = "NPM";
                        context.ReplaceInputPath = "package.json";
                        context.OutputPath = "package.json";
                    }
                },
                { "npmns=", "NPM dependency prefix namespaces", v =>
                    {
                        context.NpmPrefixes = v.Split(',').Select(n => n.Trim()).ToArray();
                        context.Language = "NPM";
                        context.ReplaceInputPath = "package.json";
                        context.OutputPath = "package.json";
                    }
                },
                { "dryrun", "dryrun mode", _ => context.IsDryRun = true },
                { "launchDebugger", "Launch debugger", _ => launchDebugger = true },
                { "help", "help", v => help = v != null },
            };

            var trails = options.Parse(args);

            if (launchDebugger)
            {
                Debugger.Launch();
            }

            if (help || (trails.Count < 1))
            {
                logger.Error($"RelaxVersioner [{ThisAssembly.AssemblyInformationalVersion}] [{ThisAssembly.AssemblyFileVersion}]");
                logger.Error("Usage: rv [options...] <projectDirectory>");
                options.WriteOptionDescriptions(Console.Error);
                logger.Error("");
                logger.Error($"Supported languages: {string.Join(", ", processor.Languages.OrderBy(l => l, StringComparer.Ordinal))}");
                logger.Error("");
                return 1;
            }

            context.ProjectDirectory = trails[0];

            var result = await processor.RunAsync(context, default);

            if (!string.IsNullOrWhiteSpace(resultPath))
            {
                ResultWriter.Write(resultPath!, result);
            }
            
            if (context.Language switch
                {
                    "Text" => context.IsDryRun,
                    "Replace" => context.IsDryRun,
                    "NPM" => context.IsDryRun,
                    _ => true,
                })
            {
                var dryrunDisplay = context.IsDryRun ?
                    " (dryrun)" : string.Empty;
                var languageDisplay = context.IsDryRun ?
                    string.Empty : $"Language={context.Language}, ";
                var tfmDisplay = context.IsDryRun ?
                    string.Empty : $"TFM={context.TargetFramework}, ";

                logger.Message(
                    LogImportance.High,
                    "Generated versions code{0}: {1}{2}Version={3}",
                    dryrunDisplay,
                    languageDisplay,
                    tfmDisplay,
                    result.Version);
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Unknown exception occurred, {0}", ex.StackTrace);
            return Marshal.GetHRForException(ex);
        }

        return 0;
    }
}
