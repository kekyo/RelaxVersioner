////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version information inserter.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

#nullable enable

using System.IO;

namespace RelaxVersioner.Writers;

internal sealed class VisualBasicWriter : WriterBase
{
    public override string Language => "VB";

    protected override void WriteComment(SourceCodeWriter tw, string format, params object?[] args) =>
        tw.WriteLine("' " + format, args);

    protected override void WriteImport(SourceCodeWriter tw, string namespaceName) =>
        tw.WriteLine("Imports {0}", namespaceName);

    protected override string GetArgumentString(string argumentValue) =>
        string.Format("\"{0}\"", argumentValue.Replace("\"", "\"\""));

    protected override void WriteAttribute(SourceCodeWriter tw, string name, string args) =>
        tw.WriteLine("<Assembly: {0}({1})>", name, args);

    protected override void WriteBeforeLiteralBody(SourceCodeWriter tw)
    {
        if (!string.IsNullOrWhiteSpace(tw.Context.Namespace))
        {
            tw.WriteLine("Namespace global.[{0}]", tw.Context.Namespace);
            tw.Shift();
        }
        tw.WriteLine("Module ThisAssembly");
        tw.Shift();
    }

    protected override void WriteBeforeNestedLiteralBody(SourceCodeWriter tw, string name)
    {
        tw.WriteLine("Public NotInheritable Class {0}", name);
        tw.Shift();
    }

    protected override void WriteLiteral(SourceCodeWriter tw, string name, string value) =>
        tw.WriteLine("Public Const [{0}] As String = {1}", name, value);

    protected override void WriteAfterNestedLiteralBody(SourceCodeWriter tw)
    {
        tw.UnShift();
        tw.WriteLine("End Class");
    }

    protected override void WriteAfterLiteralBody(SourceCodeWriter tw)
    {
        tw.UnShift();
        tw.WriteLine("End Module");

        if (!string.IsNullOrWhiteSpace(tw.Context.Namespace))
        {
            tw.UnShift();
            tw.WriteLine("End Namespace");
        }
    }

    protected override void WriteAfterBody(SourceCodeWriter tw)
    {
        var required = IsRequiredSelfHostingMetadataAttribute(tw.Context);
        if (!required)
        {
            tw.WriteLine("#if NET10 || NET11 || NET20 || NET30 || NET35 || NET40");
            tw.WriteLine();
        }

        tw.WriteLine("Namespace global.System.Reflection");
        tw.Shift();
        tw.WriteLine("<AttributeUsage(AttributeTargets.Assembly, AllowMultiple := True, Inherited := False)>");
        tw.WriteLine("Friend NotInheritable Class AssemblyMetadataAttribute");
        tw.Shift();
        tw.WriteLine("Inherits Attribute");
        tw.WriteLine("Public Sub New(key As String, value As String)");
        tw.Shift();
        tw.WriteLine("Me.Key = key");
        tw.WriteLine("Me.Value = value");
        tw.UnShift();
        tw.WriteLine("End Sub");
        tw.WriteLine("Public ReadOnly Property Key As String");
        tw.WriteLine("Public ReadOnly Property Value As String");
        tw.UnShift();
        tw.WriteLine("End Class");
        tw.UnShift();
        tw.WriteLine("End Namespace");

        if (!required)
        {
            tw.WriteLine();
            tw.WriteLine("#endif");
        }

        tw.WriteLine();
    }
}
