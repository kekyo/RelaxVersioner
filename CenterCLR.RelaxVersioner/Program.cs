////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Mono.Options;

namespace RelaxVersioner
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var relaxVersionerVersion = ThisAssembly.AssemblyVersion;
            var logger = Logger.Create($"RelaxVersioner [{relaxVersionerVersion}]", LogImportance.Normal, Console.Out, Console.Error, Console.Error);

            try
            {
                var processor = new Processor(logger);
                var languages = string.Join("|", processor.Languages);

                var context = new ProcessorContext
                {
                    Language = "C#",
                    GenerateStatic = true,
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
                    { "outputPath=", $"output source file", v => context.OutputPath = v },
                    { "resultPath=", $"output result via xml file", v => resultPath = v },
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
                    logger.Error("Usage: rv [options...] <projectDirectory>");
                    options.WriteOptionDescriptions(Console.Error);
                    return 1;
                }

                context.ProjectDirectory = trails[0];

                var result = processor.Run(context);

                var dryrunDisplay = string.IsNullOrWhiteSpace(context.OutputPath) ?
                    " (dryrun)" : string.Empty;
                var languageDisplay = string.IsNullOrWhiteSpace(context.OutputPath) ?
                    string.Empty : $"Language={context.Language}, ";
                var tfmDisplay = string.IsNullOrWhiteSpace(context.TargetFramework) ?
                    string.Empty : $"TFM={context.TargetFramework}, ";

                if (!string.IsNullOrWhiteSpace(resultPath))
                {
                    ResultWriter.Write(resultPath!, result);
                }

                logger.Message(
                    LogImportance.High,
                    "Generated versions code{0}: {1}{2}Version={3}",
                    dryrunDisplay,
                    languageDisplay,
                    tfmDisplay,
                    result.Version);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unknown exception occurred, {0}", ex.StackTrace);
                return Marshal.GetHRForException(ex);
            }

            return 0;
        }
    }
}
