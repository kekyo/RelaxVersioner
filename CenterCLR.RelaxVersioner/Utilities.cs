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
using System.Text;
using System.Xml.Linq;
using LibGit2Sharp;

namespace CenterCLR.RelaxVersioner
{
	internal static class Utilities
	{
		private static int GetVersionNumber(string stringValue)
		{
			Debug.Assert(stringValue != null);

			ushort value;
			return ushort.TryParse(stringValue, out value) ? value : -1;
		}

		public static string GetVersionFromGitLabel(string label)
		{
			Debug.Assert(label != null);

			var splittedLast = label.
				Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).
				LastOrDefault();
			if (string.IsNullOrWhiteSpace(splittedLast))
			{
				return null;
			}

			var splittedVersionElements = splittedLast.
				Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).
				Select(GetVersionNumber).
				ToList();
			if ((splittedVersionElements.Count <= 1) ||
			    splittedVersionElements.Any(elementValue => elementValue == -1))
			{
				return null;
			}

			return string.Join(".", splittedVersionElements);
		}

		public static TValue GetValue<TKey, TValue>(
			this Dictionary<TKey, TValue> dictionary,
			TKey key)
		{
			Debug.Assert(dictionary != null);
			Debug.Assert(key != null);

			TValue value;
			dictionary.TryGetValue(key, out value);
			return value;
		}

		public static string GetSafeVersionFromDate(DateTimeOffset date)
		{
			// Second range: 0..43200 (2sec prec.)
			return $"{date.Year}.{date.Month}.{date.Day}.{(ushort)(date.TimeOfDay.TotalSeconds / 2)}";
		}

		public static XElement LoadRuleSet(string path)
		{
			Debug.Assert(path != null);

			try
			{
				return XElement.Load(Path.Combine(path, "RelaxVersioner.rules"));
			}
			catch
			{
				return null;
			}
		}

		public static Dictionary<string, IEnumerable<Rule>> AggregateRuleSets(
			params XElement[] ruleSets)
		{
			Debug.Assert(ruleSets != null);

			return
				(from ruleSet in ruleSets
				 where (ruleSet != null) && (ruleSet.Name.LocalName == "RelaxVersioner")
				 from rules in ruleSet.Elements("Rules")
				 from language in rules.Elements("Language")
				 where !string.IsNullOrWhiteSpace(language?.Value)
				 select new { language, rules }).
				GroupBy(
					entry => entry.language.Value.Trim(),
					entry => entry.rules,
					StringComparer.InvariantCultureIgnoreCase).
				ToDictionary(
					g => g.Key,
					g => from rule in g.Elements("Rule")
						 let name = rule.Attribute("name")
						 let key = rule.Attribute("key")
						 where !string.IsNullOrWhiteSpace(name?.Value)
						 select new Rule(name.Value.Trim(), key?.Value.Trim(), rule.Value.Trim()),
					StringComparer.InvariantCultureIgnoreCase);
		}

		public static IEnumerable<string> AggregateNamespacesFromRuleSet(
			IEnumerable<Rule> ruleSet)
		{
			Debug.Assert(ruleSet != null);

			return
				(from rule in ruleSet
				 let symbolElements = rule.Name.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries)
				 select string.Join(".", symbolElements.Take(symbolElements.Length - 1))).
				Distinct();
		}

		public static XElement GetDefaultRuleSet()
		{
			var type = typeof(Utilities);
			using (var stream = type.Assembly.GetManifestResourceStream(
				type, "DefaultRuleSet.rules"))
			{
				return XElement.Load(stream);
			}
		}

		public static string GetLabelWithFallback(
			Branch branch,
			Dictionary<string, IEnumerable<Tag>> tags)
		{
			Debug.Assert(branch != null);
			Debug.Assert(tags != null);

			return branch.Commits.
				Select(commit => tags.GetValue(commit.Sha)).
				Where(tagList => tagList != null).
				SelectMany(tagList => tagList).
				Select(tag => GetVersionFromGitLabel(tag.Name)).
				FirstOrDefault(tagName => tagName != null) ??
				GetVersionFromGitLabel(branch.Name);
		}

		public static string FormatByRule(string ruleFormat, IEnumerable<KeyValuePair<string, object>> keyValues)
		{
			var arguments = new List<object>();
			var sb = new StringBuilder();

			var position = 0;
			while (position < ruleFormat.Length)
			{
				var index = ruleFormat.IndexOf('{', position);
				if (index == -1)
				{
					sb.Append(ruleFormat.Substring(position));
					break;
				}

				sb.Append(ruleFormat.Substring(position, index - position + 1));
				position++;

				foreach (var keyValue in keyValues)
				{
					var keyIndex = ruleFormat.IndexOf(keyValue.Key, position);
					if (keyIndex == position)
					{
						sb.Append(arguments.Count);
						position += keyValue.Key.Length;

						if ((position < ruleFormat.Length) && (ruleFormat[position] == '.'))
						{
							position++;
							var endPosition = ruleFormat.IndexOfAny(new [] { '}', ':' }, position);
							if (endPosition > position)
							{
								var propertyName = ruleFormat.Substring(position, endPosition - position);
								var type = keyValue.Value.GetType();
								var pi = type.GetProperty(propertyName);
								if ((pi != null) && pi.CanRead && (pi.GetIndexParameters().Length == 0) && (pi.GetGetMethod() != null))
								{
									var value = pi.GetValue(keyValue.Value, null);
									arguments.Add(value);
								}

								position = endPosition;
							}
						}
						else
						{
							arguments.Add(keyValue.Value);
						}

						break;
					}
				}
			}

			return string.Format(sb.ToString(), arguments.ToArray());
		}
	}
}
