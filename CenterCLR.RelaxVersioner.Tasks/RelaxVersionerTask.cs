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
using CenterCLR.RelaxVersioner.Loader;

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
        public ITaskItem OutputPath
        {
            get; set;
        }

        public string Language
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
            try
            {
                var projectDirectory = Path.GetDirectoryName(this.ProjectPath.ItemSpec);
                var language = this.Language ?? "C#";
                var isDryRun = this.Language == null;
                var outputPath = this.OutputPath.ItemSpec;

                base.Log.LogMessage(
                    MessageImportance.Normal,
                    $"RelaxVersioner: assembly base path: Managed={AssemblyLoadHelper.BasePath}, Native={AssemblyLoadHelper.BaseNativePath}");

                var result = (string[])AssemblyLoadContext.Invoke<Versioner>("Run", projectDirectory, outputPath, language, isDryRun);

                this.DetectedIdentity = result[0];
                this.DetectedShortIdentity = result[1];
                this.DetectedMessage = result[2];

                var dryrunDisplay = isDryRun ? " (dryrun)" : string.Empty;
                var languageDisplay = isDryRun ? string.Empty : $"Language={language}, ";
                base.Log.LogMessage(
                    MessageImportance.High,
                    $"RelaxVersioner: Generated versions code{dryrunDisplay}: {languageDisplay}Version={this.DetectedIdentity}, ShortVersion={this.DetectedShortIdentity}");
            }
            catch (Exception ex)
            {
                base.Log.LogErrorFromException(ex, true, true, null);
                return false;
            }

            return true;
        }
    }
}
