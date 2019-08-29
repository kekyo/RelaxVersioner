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
using System.IO;

using Microsoft.Build.Framework;

namespace CenterCLR.RelaxVersioner
{
    public sealed class RelaxVersionerTask : Microsoft.Build.Utilities.Task
    {
        public RelaxVersionerTask()
        {
        }

        [Required]
        public ITaskItem ProjectPath
        {
            get; set;
        }

        [Required]
        public ITaskItem OutputFilePath
        {
            get; set;
        }

        public string Language
        {
            get; set;
        }

        public string BuildIdentifier
        {
            get; set;
        }

        [Output]
        public string DetectedIdentity
        {
            get; set;
        }

        [Output]
        public string DetectedShortIdentity
        {
            get; set;
        }

        [Output]
        public string DetectedMessage
        {
            get; set;
        }

        public override bool Execute()
        {
            var logger = new TaskLogger(this.Log);

            logger.Message(
                LogImportance.Normal,
                "ManagedBasePath={0}",
                AssemblyLoadHelper.BasePath);

            AssemblyLoadHelper.Initialize(logger);

            logger.Message(
                LogImportance.Normal,
                "NativeRuntimeIdentifier={0}, NativeLibraryPath={1}",
                AssemblyLoadHelper.NativeRuntimeIdentifier,
                AssemblyLoadHelper.NativeLibraryPath);

            try
            {
                var projectDirectory = Path.GetDirectoryName(this.ProjectPath.ItemSpec);
                var language = this.Language ?? "C#";
                var buildIdentifier = string.IsNullOrWhiteSpace(this.BuildIdentifier) ? string.Empty : this.BuildIdentifier;
                var isDryRun = this.Language == null;
                var outputFilePath = this.OutputFilePath.ItemSpec;

                var processor = new Processor(logger);
                var result = processor.Run(projectDirectory, outputFilePath, language, buildIdentifier, isDryRun);

                this.DetectedIdentity = result.Identity;
                this.DetectedShortIdentity = result.ShortIdentity;
                this.DetectedMessage = result.Message;

                var dryrunDisplay = isDryRun ? " (dryrun)" : string.Empty;
                var languageDisplay = isDryRun ? string.Empty : $"Language={language}, ";

                logger.Message(
                    LogImportance.High,
                    "Generated versions code{0}: {1}Version={2}, ShortVersion={3}",
                    dryrunDisplay,
                    languageDisplay,
                    this.DetectedIdentity,
                    this.DetectedShortIdentity);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unknown exception occurred.");
                return false;
            }

            return true;
        }
    }
}
