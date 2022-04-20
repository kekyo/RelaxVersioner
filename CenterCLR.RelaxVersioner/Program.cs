/////////////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016-2021 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using Mono.Options;

namespace RelaxVersioner
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var relaxVersionerVersion = ThisAssembly.AssemblyVersion;
            var logger = Logger.Create($"RelaxVersioner[{relaxVersionerVersion}]", LogImportance.Normal, Console.Out, Console.Error, Console.Error);

            try
            {
                var processor = new Processor(logger);
                var languages = string.Join("|", processor.Languages);

                var context = new ProcessorContext
                {
                    Language = "C#",
                    GenerateStatic = true,
                };

                string resultPath = null;
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
                    { "help", "help", v => help = v != null },
                };

                var trails = options.Parse(args);
                if (help || (trails.Count < 1))
                {
                    logger.Error("Usage: rv [options...] <projectDirectory>");
                    options.WriteOptionDescriptions(Console.Error);
                    return 1;
                }

                context.ProjectDirectory = trails[0];

                var result = processor.Run(context);

                var dryrunDisplay = string.IsNullOrWhiteSpace(context.OutputPath) ? " (dryrun)" : string.Empty;
                var languageDisplay = string.IsNullOrWhiteSpace(context.OutputPath) ? string.Empty : $"Language={context.Language}, ";

                if (!string.IsNullOrWhiteSpace(resultPath))
                {
                    ResultWriter.Write(resultPath, result);
                }

                logger.Message(
                    LogImportance.High,
                    "Generated versions code{0}: {1}TFM={2}, Version={3}",
                    dryrunDisplay,
                    languageDisplay,
                    context.TargetFramework,
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
