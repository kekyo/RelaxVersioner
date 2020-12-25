/////////////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
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

using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RelaxVersioner
{
    public sealed class DumpMSBuildPropertiesTask : Task
    {
        [Required]
        public string OutputPath
        {
            get;
            set;
        }

        public override bool Execute()
        {
            var project = new Project(this.BuildEngine.ProjectFileOfTaskNode);
            using (var fs = File.Create(OutputPath))
            {
                var root = new XElement("Properties",
                  project.AllEvaluatedProperties.
                  Where(p => !p.IsEnvironmentProperty && !p.IsGlobalProperty && !p.IsReservedProperty).
                  Select(p => new XElement(p.Name, p.EvaluatedValue)));
                root.Save(fs);
                fs.Flush();
            }

            return true;
        }
    }
}
