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
using System.Linq;

namespace CenterCLR.RelaxVersioner.Writers
{
	internal static class Utilities
	{
		private static int GetVersionNumber(string stringValue)
		{
			ushort value;
			return UInt16.TryParse(stringValue, out value) ? value : -1;
		}

		public static string GetVersionFromGitLabel(string label)
		{
			var splittedLast = label.
				Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).
				LastOrDefault();
			if (String.IsNullOrWhiteSpace(splittedLast))
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

			return String.Join(".", splittedVersionElements);
		}

		public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
		{
			TValue value;
			dictionary.TryGetValue(key, out value);
			return value;
		}
	}
}
