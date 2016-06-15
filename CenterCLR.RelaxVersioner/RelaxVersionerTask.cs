/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016 Kouji Matsui (@kekyo2)
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
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Build.Framework;

namespace CenterCLR.RelaxVersioner
{
    public sealed class RelaxVersionerTask : ITask
    {
        #region ITask
        public IBuildEngine BuildEngine { get; set; }

        public ITaskHost HostObject { get; set; }
        #endregion

        #region Task properties
        [Required]
        public string SolutionDirectory { get; set; }

        [Required]
        public string ProjectDirectory { get; set; }

        [Required]
        public string TargetPath { get; set; }

        [Required]
        public string TargetFrameworkVersion { get; set; }

        [Required]
        public string TargetFrameworkProfile { get; set; }

        [Required]
        public string Language { get; set; }

        [Output]
        public string[] CombineDefinitions { get; set; }
        #endregion

        #region Logger
        private void LogMessage(string message)
        {
            var engine = this.BuildEngine;
            if (engine == null)
            {
                Trace.WriteLine(message);
                return;
            }

            engine.LogMessageEvent(new BuildMessageEventArgs(
                message, "", this.GetType().FullName, MessageImportance.Normal));
        }

        private void LogError(string message)
        {
            var engine = this.BuildEngine;
            if (engine == null)
            {
                Trace.WriteLine(message);
                return;
            }

            engine.LogMessageEvent(new BuildMessageEventArgs(
                message, "", this.GetType().FullName, MessageImportance.High));
        }
        #endregion

        #region Execute
        public bool Execute()
        {
            try
            {
                var solutionDirectory = this.SolutionDirectory;
                var projectDirectory = this.ProjectDirectory;
                var targetPath = this.TargetPath;
                var targetFrameworkVersion = Utilities.GetFrameworkVersionNumber(this.TargetFrameworkVersion);
                var targetFrameworkProfile = this.TargetFrameworkProfile;
                var language = this.Language;

                var combineDefinitions = new List<string>();

                var result = Executor.Execute(
                    solutionDirectory,
                    projectDirectory,
                    targetPath,
                    targetFrameworkVersion,
                    targetFrameworkProfile,
                    language,
                    combineDefinitions,
                    this.LogMessage);

                this.CombineDefinitions = combineDefinitions.ToArray();

                return result == 0;
            }
            catch (Exception ex)
            {
                this.LogError("RelaxVersioner: " + ex.Message);
                return false;
            }
        }
        #endregion
    }
}
