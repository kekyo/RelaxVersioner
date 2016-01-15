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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace CenterCLR.RelaxVersioner.Writers
{
    internal abstract class WriterBase
    {
        public abstract string Language { get; }

        public virtual void Write(
			string targetPath,
			Branch branch,
			Dictionary<string, IEnumerable<Tag>> tags,
			bool requireMetadataAttribute,
            DateTime generatedDate)
        {
            Debug.Assert(string.IsNullOrWhiteSpace(targetPath) == false);
            Debug.Assert(branch != null);
            Debug.Assert(tags != null);

            var currentCommit = branch.Commits.FirstOrDefault();
            if (currentCommit == null)
            {
                throw new InvalidOperationException("No commits.");
            }

            using (var tw = File.CreateText(targetPath))
            {
                this.WriteComment(tw, "This is auto-generated version information attributes by CenterCLR.RelaxVersioner.");
                this.WriteComment(tw, "Do not edit.");
                this.WriteComment(tw, "Generated date: {0:R}", generatedDate);
                tw.WriteLine();

                this.WriteBeforeBody(tw, requireMetadataAttribute);

				this.WriteUsing(tw, "System");
				this.WriteUsing(tw, "System.Reflection");
                tw.WriteLine();

                var commitId = currentCommit.Sha;
                var author = currentCommit.Author;
                var committer = currentCommit.Committer;
                var message = currentCommit.MessageShort;

                var fileVersion = this.GetFileVersionFromDate(committer.When);

                var version = branch.Commits.
					Select(commit => tags.GetValue(commit.Sha)).
					Where(tagList => tagList != null).
					SelectMany(tagList => tagList).
					Select(tag => Utilities.GetVersionFromGitLabel(tag.Name)).
					FirstOrDefault(tagName => tagName != null) ??
					Utilities.GetVersionFromGitLabel(branch.Name) ??
					fileVersion;

                this.WriteAttributeWithArguments(tw, "AssemblyVersionAttribute", version);
                this.WriteAttributeWithArguments(tw, "AssemblyFileVersionAttribute", fileVersion);
                this.WriteAttributeWithArguments(tw, "AssemblyInformationalVersionAttribute", commitId);

                this.WriteAttributeWithArguments(tw, "AssemblyMetadataAttribute", "Build", $"{committer.When:R}");
                this.WriteAttributeWithArguments(tw, "AssemblyMetadataAttribute", "Branch", branch.Name);
                this.WriteAttributeWithArguments(tw, "AssemblyMetadataAttribute", "Author", author);
                this.WriteAttributeWithArguments(tw, "AssemblyMetadataAttribute", "Committer", committer);
                this.WriteAttributeWithArguments(tw, "AssemblyMetadataAttribute", "Message", message);
				tw.WriteLine();

				this.WriteAfterBody(tw, requireMetadataAttribute);

                tw.Flush();
            }
        }

        protected string GetFileVersionFromDate(DateTimeOffset date)
        {
            // Second range: 0..43200 (2sec prec.)
            return $"{date.Year}.{date.Month}.{date.Day}.{(ushort)(date.TimeOfDay.TotalSeconds / 2)}";
        }

        protected virtual void WriteComment(TextWriter tw, string format, params object[] args)
        {
            tw.WriteLine("// " + format, args);
        }

        protected virtual void WriteBeforeBody(TextWriter tw, bool requireMetadataAttribute)
        {
        }

        protected abstract void WriteAttribute(TextWriter tw, string attributeName, string args);

        private void WriteAttributeWithArguments(TextWriter tw, string attributeName, params object[] args)
        {
            this.WriteAttribute(
                tw,
                attributeName,
                string.Join(",", args.Select(arg => string.Format("\"{0}\"", arg))));
        }

        protected abstract void WriteUsing(TextWriter tw, string namespaceName);

        protected virtual void WriteAfterBody(TextWriter tw, bool requireMetadataAttribute)
        {
        }
    }
}
