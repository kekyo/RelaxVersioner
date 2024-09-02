////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using NamingFormatter;

namespace RelaxVersioner.Writers;

internal sealed class TextWriteProvider : WriteProviderBase
{
    public override string Language => "Text";

    public override void Write(
        ProcessorContext context,
        Dictionary<string, object?> keyValues,
        DateTimeOffset generated,
        IEnumerable<Rule> ruleSet,
        IEnumerable<string> importSet)
    {
        Debug.Assert(string.IsNullOrWhiteSpace(context.OutputPath) == false);

        var targetFolder = Utilities.GetDirectoryPath(context.OutputPath);
        if (!string.IsNullOrWhiteSpace(targetFolder) && !Directory.Exists(targetFolder))
        {
            try
            {
                // Construct sub folders (ex: obj\Debug).
                // May fail if parallel-building on MSBuild, ignoring exceptions.
                Directory.CreateDirectory(targetFolder);
            }
            catch
            {
            }
        }

        Processor.WriteSafeTransacted(
            context.OutputPath,
            stream =>
            {
                var tw = new SourceCodeWriter(new StreamWriter(stream), context);

                foreach (var rule in ruleSet)
                {
                    var formattedValue = Named.Format(
                        CultureInfo.InvariantCulture,
                        rule.Format,
                        keyValues,
                        key => string.Empty);
                    tw.WriteLine(formattedValue);
                }

                tw.Flush();
            });
    }
}
