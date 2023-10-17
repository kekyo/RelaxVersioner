////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

#nullable enable

using System.Diagnostics;
using System.IO;

namespace RelaxVersioner.Writers;

internal sealed class SourceCodeWriter
{
    private readonly TextWriter tw;
    private int indentLevel;
    private string indent = string.Empty;

    public readonly ProcessorContext Context;

    public SourceCodeWriter(TextWriter tw, ProcessorContext context)
    {
        this.Context = context;
        this.tw = tw;
    }

    public void WriteLine() =>
        this.tw.WriteLine();

    public void WriteLine(string code) =>
        this.tw.WriteLine(indent + code);

    public void WriteLine(string code, params object?[] args) =>
        this.tw.WriteLine(indent + code, args);

    public void Flush() =>
        this.tw.Flush();

    public void Shift()
    {
        this.indentLevel++;
        this.indent = new string(' ', this.indentLevel * 4);
    }

    public void UnShift()
    {
        Debug.Assert(this.indentLevel >= 1);
        this.indentLevel--;
        this.indent = new string(' ', this.indentLevel * 4);
    }
}
