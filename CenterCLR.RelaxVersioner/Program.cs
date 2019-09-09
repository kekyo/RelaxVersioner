/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016-2019 Kouji Matsui (@kozy_kekyo, @kekyo2)
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

namespace CenterCLR.RelaxVersioner
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var relaxVersionerVersion = typeof(Program).Assembly.GetName().Version;
            var logger = Logger.Create($"RelaxVersioner[{relaxVersionerVersion}]", LogImportance.Normal, Console.Out, Console.Error, Console.Error);

            try
            {
                var processor = new Processor(logger);
                var languages = string.Join("|", processor.Languages);

                var language = "C#";
                string buildIdentifier = null;
                var isDryRun = false;
                var isHelp = false;

                var options = new OptionSet
                {
                    { "l=", $"target language [{languages}]", v => language = v },
                    { "i=", $"build identifier", v => buildIdentifier = v },
                    { "d", "dry run mode", v => isDryRun = v != null },
                    { "h", "help", v => isHelp = v != null },
                };

                var trails = options.Parse(args);
                if (isHelp || (trails.Count < 2))
                {
                    logger.Error("Usage: rv [options...] <projectDirectory> <outputFilePath>");
                    options.WriteOptionDescriptions(Console.Error);
                    return 1;
                }

                var projectDirectory = trails[0];
                var outputFilePath = trails[1];

                var result = processor.Run(
                    projectDirectory, outputFilePath, language, buildIdentifier, isDryRun);

                var dryrunDisplay = isDryRun ? " (dryrun)" : string.Empty;
                var languageDisplay = isDryRun ? string.Empty : $"Language={language}, ";

                logger.Message(
                    LogImportance.High,
                    "Generated versions code{0}: {1}Version={2}",
                    dryrunDisplay,
                    languageDisplay,
                    result.Identity);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unknown exception occurred.");
                return Marshal.GetHRForException(ex);
            }

            return 0;
        }
    }
}
