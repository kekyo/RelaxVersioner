////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using NamingFormatter;

namespace RelaxVersioner.Writers;

internal sealed class TextWriteProvider : WriteProviderBase
{
    public override string Language => "Text";

    public override void Write(
        ProcessorContext context,
        Dictionary<string, object?> keyValues,
        DateTimeOffset generated)
    {
        void Write(TextWriter tw, bool emitEol)
        {
            var scw = new SourceCodeWriter(tw, context);

            var formattedValue = Named.Format(
                CultureInfo.InvariantCulture,
                context.TextFormat,
                keyValues,
                key => string.Empty,
                new(context.BracketStart, context.BracketEnd));

            if (emitEol)
            {
                scw.WriteLine(formattedValue);
            }
            else
            {
                scw.WriteRaw(formattedValue);
            }

            scw.Flush();
        }
        
        if (!string.IsNullOrWhiteSpace(context.OutputPath))
        {
            if (context.IsDryRun)
            {
                return;
            }

            Processor.WriteSafeTransacted(
                context.OutputPath,
                stream => Write(new StreamWriter(stream, Utilities.UTF8), false));
        }
        else
        {
            if (context.IsQuietOnStandardOutput)
            {
                return;
            }

            var tw = Console.Out;
            Write(tw, true);
        }
    }
}
