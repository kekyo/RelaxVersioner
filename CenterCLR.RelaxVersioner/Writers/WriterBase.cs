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
			DateTime generated,
			IEnumerable<Rule> ruleSet)
		{
			Debug.Assert(string.IsNullOrWhiteSpace(targetPath) == false);
			Debug.Assert(branch != null);
			Debug.Assert(tags != null);
			Debug.Assert(ruleSet != null);

			var commit = branch.Commits.FirstOrDefault();
			if (commit == null)
			{
				throw new InvalidOperationException("No commits.");
			}

			using (var tw = File.CreateText(targetPath))
			{
				this.WriteComment(tw, "This is auto-generated version information attributes by CenterCLR.RelaxVersioner.{0}",
					this.GetType().Assembly.GetName().Version);
				this.WriteComment(tw, "Do not edit.");
				this.WriteComment(tw, "Generated date: {0:R}", generated);
				tw.WriteLine();

				this.WriteBeforeBody(tw, requireMetadataAttribute);

				var namespaces = Utilities.AggregateNamespacesFromRuleSet(ruleSet);
				foreach (var namespaceName in namespaces)
				{
					this.WriteImport(tw, namespaceName);
				}
				tw.WriteLine();

				var commitId = commit.Sha;
				var author = commit.Author;
				var committer = commit.Committer;

				var safeVersion = Utilities.GetSafeVersionFromDate(committer.When);
				var gitLabel = Utilities.GetLabelWithFallback(branch, tags) ?? safeVersion;

				var keyValues = new SortedList<string, object>(new StringLengthDescComparer());
				keyValues.Add("generated", generated);
				keyValues.Add("branch", branch);
				keyValues.Add("commit", commit);
				keyValues.Add("author", author);
				keyValues.Add("committer", committer);
				keyValues.Add("commitId", commitId);
				keyValues.Add("gitLabel", gitLabel);
				keyValues.Add("safeVersion", safeVersion);

				foreach (var rule in ruleSet)
				{
					var formattedValue = Utilities.FormatByRule(rule.Format, keyValues);
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

				this.WriteAfterBody(tw, requireMetadataAttribute);

				tw.Flush();
			}
		}

		protected virtual void WriteComment(TextWriter tw, string format, params object[] args)
		{
			tw.WriteLine("// " + format, args);
		}

		protected virtual void WriteBeforeBody(TextWriter tw, bool requireMetadataAttribute)
		{
		}

		protected abstract void WriteAttribute(TextWriter tw, string name, string args);

		private void WriteAttributeWithArguments(TextWriter tw, string name, params object[] args)
		{
			this.WriteAttribute(
				tw,
				name,
				string.Join(",", args.Select(arg => string.Format("\"{0}\"", arg))));
		}

		protected virtual void WriteImport(TextWriter tw, string namespaceName)
		{
		}

		protected virtual void WriteAfterBody(TextWriter tw, bool requireMetadataAttribute)
		{
		}

		private sealed class StringLengthDescComparer : IComparer<string>
		{
			public int Compare(string x, string y)
			{
				return
					x.Length < y.Length
						? 1
						: (x.Length > y.Length) ? -1
						: x.CompareTo(y);
			}
		}
	}
}
