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
    internal sealed class FSharpWriter : WriterBase
    {
        public override string Language => "F#";

        protected override void WriteBeforeBody(SourceCodeWriter tw)
        {
            var required = IsRequiredSelfHostingMetadataAttribute(tw.Context);
            if (!required)
            {
                tw.WriteLine("#if NET20 || NET30 || NET35 || NET40");
                tw.WriteLine();
            }

            tw.WriteLine("namespace System.Reflection");
            tw.Shift();
            tw.WriteLine("open System");
            tw.WriteLine("[<Sealed>]");
            tw.WriteLine("[<NoEquality>]");
            tw.WriteLine("[<NoComparison>]");
            tw.WriteLine("[<AutoSerializable(false)>]");
            tw.WriteLine("[<AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)>]");
            tw.WriteLine("type internal AssemblyMetadataAttribute(key: string, value: string) =");
            tw.Shift();
            tw.WriteLine("inherit Attribute()");
            tw.WriteLine("member this.Key = key");
            tw.WriteLine("member this.Value = value");
            tw.UnShift();
            tw.UnShift();

            if (!required)
            {
                tw.WriteLine();
                tw.WriteLine("#endif");
            }

            tw.WriteLine();
            tw.WriteLine("namespace global");
            tw.Shift();
        }

        protected override void WriteImport(SourceCodeWriter tw, string namespaceName) =>
            tw.WriteLine("open {0}", namespaceName);
        
        protected override string GetArgumentString(string argumentValue) =>
            string.Format("@\"{0}\"", argumentValue.Replace("\"", "\"\""));

        protected override void WriteAttribute(SourceCodeWriter tw, string name, string args) =>
            tw.WriteLine("[<assembly: {0}({1})>]", name, args);

        protected override void WriteLiteral(SourceCodeWriter tw, string name, string value)
        {
            tw.WriteLine("[<Literal>]");
            tw.WriteLine("let ``{0}`` = {1};", name, value);
        }

        protected override void WriteBeforeLiteralBody(SourceCodeWriter tw)
        {
            tw.WriteLine("do()");
            tw.UnShift();
            tw.WriteLine();

            if (!string.IsNullOrWhiteSpace(tw.Context.Namespace))
            {
                tw.WriteLine("namespace {0}", tw.Context.Namespace);
            }
            else
            {
                tw.WriteLine("namespace global");
            }
            tw.Shift();

            tw.WriteLine("module internal ThisAssembly =");
            tw.Shift();
        }

        protected override void WriteBeforeNestedLiteralBody(SourceCodeWriter tw, string name)
        {
            tw.WriteLine("module {0} =", name);
            tw.Shift();
        }

        protected override void WriteAfterNestedLiteralBody(SourceCodeWriter tw) =>
            tw.UnShift();

        protected override void WriteAfterLiteralBody(SourceCodeWriter tw)
        {
            tw.UnShift();
            if (!string.IsNullOrWhiteSpace(tw.Context.Namespace))
            {
                tw.UnShift();
            }
        }

        protected override void WriteAfterBody(SourceCodeWriter tw)
        {
            tw.WriteLine("do()");
            tw.UnShift();
            tw.WriteLine();
        }
    }
}
