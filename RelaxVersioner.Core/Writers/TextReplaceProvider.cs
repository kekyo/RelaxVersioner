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
using System.Text;
using NamingFormatter;

namespace RelaxVersioner.Writers;

internal sealed class TextReplaceProvider : WriteProviderBase
{
    public override string Language => "Replace";

    public override void Write(
        ProcessorContext context,
        Dictionary<string, object?> keyValues,
        DateTimeOffset generated)
    {
        void Replace(TextReader tr, TextWriter tw)
        {
            while (true)
            {
                var line = tr.ReadLine();
                if (line == null)
                {
                    break;
                }

                var formattedLine = Named.Format(
                    CultureInfo.InvariantCulture,
                    line,
                    keyValues,
                    key => string.Empty,
                    new(context.BracketStart, context.BracketEnd));

                tw.WriteLine(formattedLine);
            }

            tw.Flush();
        }
        
        if (!string.IsNullOrWhiteSpace(context.OutputPath))
        {
            if (context.IsDryRun)
            {
                return;
            }

            Processor.WriteSafeTransacted(
                context.OutputPath,
                stream =>
                {
                    using var tr = context.ReplaceInputPath is { } rip ?
                        new StreamReader(rip, Utilities.UTF8, true) :
                        Console.In;
                    var tw = new StreamWriter(stream, Utilities.UTF8);

                    Replace(tr, tw);
                });
        }
        else
        {
            if (context.IsQuietOnStandardOutput)
            {
                return;
            }

            using var tr = context.ReplaceInputPath is { } rip ?
                new StreamReader(rip, Utilities.UTF8, true) :
                Console.In;
            Replace(tr, Console.Out);
        }
    }
}
