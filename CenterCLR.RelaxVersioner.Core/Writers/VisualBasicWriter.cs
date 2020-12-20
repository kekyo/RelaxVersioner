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

using System.IO;

namespace RelaxVersioner.Writers
{
    internal sealed class VisualBasicWriter : WriterBase
    {
        public override string Language => "VB";

        protected override void WriteComment(SourceCodeWriter tw, string format, params object[] args) =>
            tw.WriteLine("' " + format, args);

        protected override void WriteImport(SourceCodeWriter tw, string namespaceName) =>
            tw.WriteLine("Imports {0}", namespaceName);

        protected override string GetArgumentString(string argumentValue) =>
            string.Format("\"{0}\"", argumentValue.Replace("\"", "\"\""));

        protected override void WriteAttribute(SourceCodeWriter tw, string name, string args) =>
            tw.WriteLine("<Assembly: {0}({1})>", name, args);
        
        protected override void WriteLiteral(SourceCodeWriter tw, string name, string value) =>
            tw.WriteLine("    Public Const {0} As String = {1}", name, value);

        protected override void WriteBeforeLiteralBody(SourceCodeWriter tw, string namespaceName)
        {
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                tw.WriteLine("Namespace {0}", namespaceName);
            }
            tw.WriteLine("  Friend NotInheritable Class ThisAssembly");
        }

        protected override void WriteBeforeNestedLiteralBody(SourceCodeWriter tw, string name)
        {
            tw.WriteLine("    Public NotInheritable Class {0}", name);
        }

        protected override void WriteAfterNestedLiteralBody(SourceCodeWriter tw) =>
            tw.WriteLine("	  End Class");

        protected override void WriteAfterLiteralBody(SourceCodeWriter tw, string namespaceName)
        {
            tw.WriteLine("	End Class");
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                tw.WriteLine("End Namespace");
            }
        }

        protected override void WriteAfterBody(SourceCodeWriter tw)
        {
            tw.WriteLine("Namespace global.System.Reflection");
            tw.WriteLine("	<AttributeUsage(AttributeTargets.Assembly, AllowMultiple := True, Inherited := False)>");
            tw.WriteLine("	Friend NotInheritable Class AssemblyVersionMetadataAttribute");
            tw.WriteLine("		Inherits Attribute");
            tw.WriteLine("		Public Sub New(key As String, value As String)");
            tw.WriteLine("			Me.Key = key");
            tw.WriteLine("			Me.Value = value");
            tw.WriteLine("		End Sub");
            tw.WriteLine("		Public ReadOnly Property Key As String");
            tw.WriteLine("		Public ReadOnly Property Value As String");
            tw.WriteLine("	End Class");
            tw.WriteLine("End Namespace");
            tw.WriteLine();
        }
    }
}
