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

using NamingFormatter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RelaxVersioner.Writers
{
    internal abstract class WriterBase
    {
        public abstract string Language { get; }

        public void Write(
            string outputFilePath,
            Dictionary<string, object> keyValues,
            DateTimeOffset generated,
            IEnumerable<Rule> ruleSet,
            IEnumerable<string> importSet)
        {
            Debug.Assert(string.IsNullOrWhiteSpace(outputFilePath) == false);
            Debug.Assert(keyValues != null);
            Debug.Assert(ruleSet != null);
            Debug.Assert(importSet != null);

            var targetFolder = Path.GetDirectoryName(outputFilePath);
            if (!string.IsNullOrWhiteSpace(targetFolder) && !Directory.Exists(targetFolder))
            {
                try
                {
                    // Construct sub folders (ex: obj\Debug).
                    // May fail if parallel-building on MSBuild, ignoring exceptions.
                    Directory.CreateDirectory(targetFolder);
                }
                catch
                {
                }
            }

            using (var tw = File.CreateText(outputFilePath))
            {
                this.WriteComment(tw,
                    $"This is auto-generated version information attributes by RelaxVersioner.{this.GetType().Assembly.GetName().Version}, Do not edit.");
                this.WriteComment(tw,
                    $"Generated date: {generated:R}");
                tw.WriteLine();

                this.WriteBeforeBody(tw);

                foreach (var namespaceName in importSet)
                {
                    this.WriteImport(tw, namespaceName);
                }
                tw.WriteLine();

                foreach (var rule in ruleSet)
                {
                    var formattedValue = Named.Format(rule.Format, keyValues);
                    if (!string.IsNullOrWhiteSpace(rule.Key))
                    {
                        this.WriteAttributeWithArguments(tw, rule.Name, rule.Key, formattedValue);
                    }
                    else
                    {
                        this.WriteAttributeWithArguments(tw, rule.Name, formattedValue);
                    }
                }
                tw.WriteLine();

                this.WriteAfterBody(tw);

                tw.Flush();
            }
        }

        protected virtual void WriteComment(TextWriter tw, string format, params object[] args)
        {
            tw.WriteLine("// " + format, args);
        }

        protected virtual void WriteBeforeBody(TextWriter tw)
        {
        }

        protected abstract void WriteAttribute(TextWriter tw, string name, string args);

        protected virtual string GetArgumentString(string argumentValue)
        {
            return string.Format("\"{0}\"", argumentValue.Replace("\"", "\"\""));
        }

        private void WriteAttributeWithArguments(TextWriter tw, string name, params object[] args)
        {
            this.WriteAttribute(
                tw,
                name,
                string.Join(",", args.Select(arg => this.GetArgumentString((arg != null) ? arg.ToString() : string.Empty))));
        }

        protected virtual void WriteImport(TextWriter tw, string namespaceName)
        {
        }

        protected virtual void WriteAfterBody(TextWriter tw)
        {
        }
    }
}
