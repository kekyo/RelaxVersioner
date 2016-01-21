/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016 Kouji Matsui (@kekyo2)
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

using System.IO;

namespace CenterCLR.RelaxVersioner.Writers
{
	internal sealed class CPlusPlusCLIWriter : WriterBase
	{
		public override string Language => "C++/CLI";

		protected override void WriteBeforeBody(TextWriter tw, bool requireMetadataAttribute)
		{
			tw.WriteLine("#include \"stdafx.h\"");
			tw.WriteLine();

			if (requireMetadataAttribute == true)
			{
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
				tw.WriteLine();
			}
		}

		protected override void WriteAttribute(TextWriter tw, string name, string args)
		{
			tw.WriteLine("[assembly: {0}({1})];", name.Replace(".", "::"), args);
		}
	}
}
