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

using System.Diagnostics;
using System.IO;

namespace RelaxVersioner.Writers
{
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

        public void WriteLine(string code, params object[] args) =>
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
}
