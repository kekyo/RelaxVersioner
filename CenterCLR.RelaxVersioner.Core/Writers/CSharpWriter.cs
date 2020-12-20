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

namespace RelaxVersioner.Writers
{
    internal sealed class CSharpWriter : WriterBase
    {
        public override string Language => "C#";

        protected override void WriteImport(SourceCodeWriter tw, string namespaceName) =>
            tw.WriteLine("using {0};", namespaceName);
        
        protected override string GetArgumentString(string argumentValue) =>
            string.Format("@\"{0}\"", argumentValue.Replace("\"", "\"\""));

        protected override void WriteAttribute(SourceCodeWriter tw, string name, string args) =>
            tw.WriteLine("[assembly: {0}({1})]", name, args);

        protected override void WriteLiteral(SourceCodeWriter tw, string name, string value) =>
            tw.WriteLine("    public const string {0} = {1};", name, value);

        protected override void WriteBeforeLiteralBody(SourceCodeWriter tw, string namespaceName)
        {
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                tw.WriteLine("namespace {0}", namespaceName);
                tw.WriteLine("{");
            }

            tw.WriteLine("internal static class ThisAssembly");
            tw.WriteLine("{");
        }

        protected override void WriteBeforeNestedLiteralBody(SourceCodeWriter tw, string name)
        {
            tw.WriteLine("    public static class {0}", name);
            tw.WriteLine("    {");
        }

        protected override void WriteAfterNestedLiteralBody(SourceCodeWriter tw) =>
            tw.WriteLine("    }");

        protected override void WriteAfterLiteralBody(SourceCodeWriter tw, string namespaceName)
        {
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                tw.WriteLine("}");
            }
            tw.WriteLine("}");
        }

        protected override void WriteAfterBody(SourceCodeWriter tw)
        {
            tw.WriteLine("namespace System.Reflection");
            tw.WriteLine("{");
            tw.WriteLine("    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]");
            tw.WriteLine("    internal sealed class AssemblyVersionMetadataAttribute : Attribute");
            tw.WriteLine("    {");
            tw.WriteLine("        public AssemblyVersionMetadataAttribute(string key, string value)");
            tw.WriteLine("        {");
            tw.WriteLine("            this.Key = key;");
            tw.WriteLine("            this.Value = value;");
            tw.WriteLine("        }");
            tw.WriteLine("        public string Key { get; private set; }");
            tw.WriteLine("        public string Value { get; private set; }");
            tw.WriteLine("    }");
            tw.WriteLine("}");
            tw.WriteLine();
        }
    }
}
