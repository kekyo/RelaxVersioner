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
    internal sealed class CPlusPlusCLIWriter : WriterBase
    {
        public override string Language => "C++/CLI";

        protected override void WriteBeforeBody(SourceCodeWriter tw)
        {
            tw.WriteLine("#include \"stdafx.h\"");
            tw.WriteLine();

            var required = IsRequiredSelfHostingMetadataAttribute(tw.Context);
            if (!required)
            {
                tw.WriteLine("#if defined(NET20) || defined(NET30) || defined(NET35) || defined(NET40)");
                tw.WriteLine();
            }

            tw.WriteLine("namespace System");
            tw.WriteLine("{");
            tw.WriteLine("	namespace Reflection");
            tw.WriteLine("	{");
            tw.WriteLine("		[System::AttributeUsage(System::AttributeTargets::Assembly, AllowMultiple = true, Inherited = false)]");
            tw.WriteLine("		private ref class AssemblyMetadataAttribute sealed : public System::Attribute");
            tw.WriteLine("		{");
            tw.WriteLine("		private:");
            tw.WriteLine("			System::String^ key_;");
            tw.WriteLine("			System::String^ value_;");
            tw.WriteLine("		public:");
            tw.WriteLine("			AssemblyMetadataAttribute(System::String^ key, System::String^ value)");
            tw.WriteLine("				: key_(key), value_(value) { }");
            tw.WriteLine("			property System::String^ Key { System::String^ get() { return key_; } }");
            tw.WriteLine("			property System::String^ Value { System::String^ get() { return value_; } }");
            tw.WriteLine("		};");
            tw.WriteLine("	}");
            tw.WriteLine("}");

            if (!required)
            {
                tw.WriteLine();
                tw.WriteLine("#endif");
            }

            tw.WriteLine();
        }

        protected override void WriteImport(SourceCodeWriter tw, string namespaceName) =>
            tw.WriteLine("using namespace {0};", namespaceName.Replace(".", "::"));
        
        protected override string GetArgumentString(string argumentValue) =>
            string.Format("\"{0}\"", argumentValue.Replace("\\", "\\\\").Replace("\"", "\\\""));

        protected override void WriteAttribute(SourceCodeWriter tw, string name, string args) =>
            tw.WriteLine("[assembly: {0}({1})];", name.Replace(".", "::"), args);

        protected override void WriteBeforeLiteralBody(SourceCodeWriter tw)
        {
            if (!string.IsNullOrWhiteSpace(tw.Context.Namespace))
            {
                tw.WriteLine("namespace {0}", tw.Context.Namespace);
                tw.WriteLine("{");
                tw.Shift();
            }

            tw.WriteLine("private ref class ThisAssembly abstract sealed");
            tw.WriteLine("{");
            tw.WriteLine("public:");
            tw.Shift();
        }

        protected override void WriteBeforeNestedLiteralBody(SourceCodeWriter tw, string name)
        {
            tw.WriteLine("ref class {0} abstract sealed", name);
            tw.WriteLine("{");
            tw.Shift();
        }

        protected override void WriteLiteral(SourceCodeWriter tw, string name, string value) =>
            tw.WriteLine("literal System::String^ __identifier({0}) = {1};", name, value);

        protected override void WriteAfterNestedLiteralBody(SourceCodeWriter tw)
        {
            tw.UnShift();
            tw.WriteLine("};");
        }

        protected override void WriteAfterLiteralBody(SourceCodeWriter tw)
        {
            if (!string.IsNullOrWhiteSpace(tw.Context.Namespace))
            {
                tw.UnShift();
                tw.WriteLine("};");
            }
            tw.UnShift();
            tw.WriteLine("};");
        }
    }
}
