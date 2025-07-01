////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

namespace RelaxVersioner.Writers;

internal sealed class CSharpSourceCodeWriteProvider : SourceCodeWriteProviderBase
{
    public override string Language => "C#";

    protected override void WriteImport(SourceCodeWriter tw, string namespaceName)
    {
        var required = IsRequiredSelfHostingMetadataAttribute(tw.Context);
        if (required)
        {
            tw.WriteLine("#pragma warning disable 436");
            tw.WriteLine();
        }
        
        tw.WriteLine("using {0};", namespaceName);
    }
    
    protected override void WriteAttribute(SourceCodeWriter tw, string name, string args) =>
        tw.WriteLine("[assembly: {0}({1})]", name, args);

    protected override void WriteBeforeLiteralBody(SourceCodeWriter tw)
    {
        if (!string.IsNullOrWhiteSpace(tw.Context.Namespace))
        {
            tw.WriteLine("namespace {0}", tw.Context.Namespace);
            tw.WriteLine("{");
            tw.Shift();
        }

        tw.WriteLine("internal static class ThisAssembly");
        tw.WriteLine("{");
        tw.Shift();
    }

    protected override void WriteBeforeNestedLiteralBody(SourceCodeWriter tw, string name)
    {
        tw.WriteLine("public static class {0}", name);
        tw.WriteLine("{");
        tw.Shift();
    }

    protected override void WriteLiteral(SourceCodeWriter tw, string name, string value) =>
        tw.WriteLine("public const string @{0} = {1};", name, value);

    protected override void WriteAfterNestedLiteralBody(SourceCodeWriter tw)
    {
        tw.UnShift();
        tw.WriteLine("}");
    }

    protected override void WriteAfterLiteralBody(SourceCodeWriter tw)
    {
        if (!string.IsNullOrWhiteSpace(tw.Context.Namespace))
        {
            tw.UnShift();
            tw.WriteLine("}");
        }
        tw.UnShift();
        tw.WriteLine("}");
    }

    protected override void WriteAfterBody(SourceCodeWriter tw)
    {
        var required = IsRequiredSelfHostingMetadataAttribute(tw.Context);
        if (!required)
        {
            tw.WriteLine("#if NET10 || NET11 || NET20 || NET30 || NET35 || NET40");
            tw.WriteLine();
        }

        tw.WriteLine("namespace System.Reflection");
        tw.WriteLine("{");
        tw.Shift();
        tw.WriteLine("[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]");
        tw.WriteLine("internal sealed class AssemblyMetadataAttribute : Attribute");
        tw.WriteLine("{");
        tw.Shift();
        tw.WriteLine("public AssemblyMetadataAttribute(string key, string value)");
        tw.WriteLine("{");
        tw.Shift();
        tw.WriteLine("this.Key = key;");
        tw.WriteLine("this.Value = value;");
        tw.UnShift();
        tw.WriteLine("}");
        tw.WriteLine("public string Key { get; private set; }");
        tw.WriteLine("public string Value { get; private set; }");
        tw.UnShift();
        tw.WriteLine("}");
        tw.UnShift();
        tw.WriteLine("}");

        if (!required)
        {
            tw.WriteLine();
            tw.WriteLine("#endif");
        }

        tw.WriteLine();
    }
}
