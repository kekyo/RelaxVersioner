////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System.IO;
using System.Linq;
using System.Xml.Linq;

using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RelaxVersioner;

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
        if (string.IsNullOrWhiteSpace(this.OutputPath))
        {
            this.Log.LogError("RelaxVersioner.DumpPropertiesTask: Required output path.");
            return false;
        }

        // Yes, it's a HACK:
        var buildRequestEntry = this.BuildEngine?.GetField("_requestEntry");

        var requestConfiguration = buildRequestEntry?.GetProperty("RequestConfiguration");
        var project = (ProjectInstance?)requestConfiguration?.GetProperty("Project");

        if (project != null)
        {
            var basePath = Utilities.GetDirectoryPath(this.OutputPath!);
            if (!Directory.Exists(basePath))
            {
                try
                {
                    Directory.CreateDirectory(basePath);
                }
                catch
                {
                }
            }

            using (var fs = File.Create(this.OutputPath))
            {
                var root = new XElement("Properties",
                    project.Properties.
                    Cast<ProjectPropertyInstance>().
                    Select(p => new XElement(p.Name, p.EvaluatedValue ?? "")));
                root.Save(fs);
                fs.Flush();
            }

            this.Log.LogMessage($"RelaxVersioner.DumpPropertiesTask: Dump properties from build engine, Path={this.OutputPath}");
            return true;
        }
        else
        {
            this.Log.LogError("RelaxVersioner.DumpPropertiesTask: Unable contact build engine.");
            return false;
        }
    }
}
