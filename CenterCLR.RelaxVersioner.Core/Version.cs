/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016-2019 Kouji Matsui (@kozy_kekyo, @kekyo2)
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

namespace CenterCLR.RelaxVersioner
{
    public struct Version
    {
        private static readonly char[] separators = new[] { '.', ',', '/', '-', '_' };

        public static readonly Version Empty = new Version(0, 0, 0);

        public readonly int Major;
        public readonly int? Minor;
        public readonly int? Build;
        public readonly int? Revision;

        public Version(int major)
        {
            this.Major = major;
            this.Minor = null;
            this.Build = null;
            this.Revision = null;
        }

        public Version(int major, int minor)
        {
            this.Major = major;
            this.Minor = minor;
            this.Build = null;
            this.Revision = null;
        }

        public Version(int major, int minor, int build)
        {
            this.Major = major;
            this.Minor = minor;
            this.Build = build;
            this.Revision = null;
        }

        public Version(int major, int minor, int build, int revision)
        {
            this.Major = major;
            this.Minor = minor;
            this.Build = build;
            this.Revision = revision;
        }

        private IEnumerable<int> GetComponents()
        {
            yield return this.Major;
            if (this.Minor.HasValue)
            {
                yield return this.Minor.Value;
                if (this.Build.HasValue)
                {
                    yield return this.Build.Value;
                    if (this.Revision.HasValue)
                    {
                        yield return this.Revision.Value;
                    }
                }
            }
        }

        public string ToString(int maxComponents, char separator = '.') =>
            string.Join(separator.ToString(),
                this.GetComponents().
                Select((component, index) => new { component, index }).
                TakeWhile(entry => entry.index < maxComponents).
                Select(entry => entry.component));

        public string ToString(char separator) =>
            string.Join(separator.ToString(), this.GetComponents());

        public override string ToString() =>
            this.ToString('.');

        public static bool TryParse(string versionString, out Version version)
        {
            var components = versionString.
                TrimStart('v').     // v1.2.3
                Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var numberComponents = components.
                Select(numberString => int.TryParse(numberString, out var number) ? (int?)number : null).
                TakeWhile(number => number.HasValue).
                ToArray();
            switch (numberComponents.Length)
            {
                case 0:
                    version = new Version();
                    return false;
                case 1:
                    version = new Version(
                        numberComponents[0].Value);
                    return true;
                case 2:
                    version = new Version(
                        numberComponents[0].Value,
                        numberComponents[1].Value);
                    return true;
                case 3:
                    version = new Version(
                        numberComponents[0].Value,
                        numberComponents[1].Value,
                        numberComponents[2].Value);
                    return true;
                default:
                    version = new Version(
                        numberComponents[0].Value,
                        numberComponents[1].Value,
                        numberComponents[2].Value,
                        numberComponents[3].Value);
                    return true;
            }
        }
    }
}
