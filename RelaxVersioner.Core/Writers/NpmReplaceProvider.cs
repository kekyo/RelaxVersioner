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
using System.Linq;
using System.Text;
using NamingFormatter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RelaxVersioner.Writers;

internal sealed class NpmReplaceProvider : WriteProviderBase
{
    public override string Language => "NPM";

    public override void Write(
        ProcessorContext context,
        Dictionary<string, object?> keyValues,
        DateTimeOffset generated)
    {
        void Replace(TextReader tr, TextWriter tw)
        {
            var jr = new JsonTextReader(tr);
            var jt = JToken.ReadFrom(jr, new JsonLoadSettings
                {
                    CommentHandling = CommentHandling.Load,
                    LineInfoHandling = LineInfoHandling.Load,
                });
            
            var formattedVersion = Named.Format(
                CultureInfo.InvariantCulture,
                context.TextFormat,
                keyValues,
                key => string.Empty,
                new(context.BracketStart, context.BracketEnd));
            
            jt["version"] = formattedVersion;

            if (context.NpmPrefixes.Length >= 1)
            {
                void ReplaceSubKey(string key)
                {
                    if (jt[key] is JObject jo)
                    {
                        foreach (var jp in jo.Properties())
                        {
                            if (context.NpmPrefixes.Any(jp.Name.StartsWith))
                            {
                                jp.Value = JValue.CreateString($"^{formattedVersion}");
                            }
                        }
                    }
                }
            
                ReplaceSubKey("dependencies");
                ReplaceSubKey("peerDependencies");
                ReplaceSubKey("devDependencies");
            }
            
            var jw = new JsonTextWriter(tw);
            var s = new JsonSerializer()
            {
                Formatting = Formatting.Indented,
            };
            s.Serialize(jw, jt);

            jw.Flush();
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
