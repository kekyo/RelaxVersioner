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
    internal sealed class CSharpWriter : WriterBase
    {
        public override string Extension => ".cs";

        protected override void WriteUsing(TextWriter tw, string namespaceName)
        {
            tw.WriteLine("using {0};", namespaceName);
        }

        protected override void WriteAttribute(TextWriter tw, string attributeName, string args)
        {
            tw.WriteLine("[assembly: {0}({1})]", attributeName, args);
        }
    }
}
