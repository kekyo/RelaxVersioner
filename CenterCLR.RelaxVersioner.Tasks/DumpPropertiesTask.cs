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

using System.IO;
using System.Linq;
using System.Xml.Linq;

using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RelaxVersioner
{
    public sealed class DumpPropertiesTask : Task
    {
        [Required]
        public string? OutputPath
        {
            get;
            set;
        }

        public override bool Execute()
        {
            if (string.IsNullOrWhiteSpace(OutputPath))
            {
                this.Log.LogError("RelaxVersioner.DumpPropertiesTask: Required output path.");
                return false;
            }

            var buildRequestEntry = this.BuildEngine?.GetField("_requestEntry");
            var requestConfiguration = buildRequestEntry?.GetProperty("RequestConfiguration");
            var project = (ProjectInstance?)requestConfiguration?.GetProperty("Project");

            if (project != null)
            {
                using (var fs = File.Create(OutputPath))
                {
                    var root = new XElement("Properties",
                        project.Properties.
                        Cast<ProjectPropertyInstance>().
                        Select(p => new XElement(p.Name, p.EvaluatedValue ?? "")));
                    root.Save(fs);
                    fs.Flush();
                }

                this.Log.LogMessage($"RelaxVersioner.DumpPropertiesTask: Dump properties from build engine, Path={OutputPath}");
                return true;
            }
            else
            {
                this.Log.LogError("RelaxVersioner.DumpPropertiesTask: Unable contact build engine.");
                return false;
            }
        }
    }
}
