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
    internal sealed class CPlusPlusWriter : WriterBase
    {
        public override string Extension => ".cpp";

        protected override void WriteUsing(TextWriter tw, string namespaceName)
        {
            tw.WriteLine("using namespace {0}", namespaceName.Replace(".", "::"));
        }

        protected override void WriteAttribute(TextWriter tw, string attributeName, string args)
        {
            tw.WriteLine("[assembly: {0}Attribute({1})];", attributeName, args);
        }

		protected override void WriteAfterBody(TextWriter tw, bool requireMetadataAttribute)
		{
			if (requireMetadataAttribute == true)
			{
				tw.WriteLine("namespace System");
				tw.WriteLine("{");
				tw.WriteLine("	namespace Reflection");
				tw.WriteLine("	{");
				tw.WriteLine("		[AttributeUsage(AttributeTargets::All, AllowMultiple = true)]");
				tw.WriteLine("		internal ref class AssemblyMetadataAttribute : public Attribute sealed");
				tw.WriteLine("		{");
				tw.WriteLine("		private:");
				tw.WriteLine("			System::String^ key_;");
				tw.WriteLine("			System::String^ value_;");
				tw.WriteLine("		public:");
				tw.WriteLine("			AssemblyMetadataAttribute(System::String^ key, System::String^ value)");
				tw.WriteLine("				: key_(key), value_(value) { }");
				tw.WriteLine("			property System::String^ Key { get() { return key_; } }");
				tw.WriteLine("			property System::String^ Value { get() { return value_; }");
				tw.WriteLine("		};");
				tw.WriteLine("	}");
				tw.WriteLine("}");
				tw.WriteLine();
			}
		}
	}
}
