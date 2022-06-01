////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace RelaxVersioner
{
    public struct Version
    {
        private static readonly char[] separators = new[] { '.', ',', '/', '-', '_' };

        public static readonly Version Default = new Version(0, 0, 1);

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
                Select(number => number!.Value).
                ToArray();
            switch (numberComponents.Length)
            {
                case 0:
                    version = new Version();
                    return false;
                case 1:
                    version = new Version(
                        numberComponents[0]);
                    return true;
                case 2:
                    version = new Version(
                        numberComponents[0],
                        numberComponents[1]);
                    return true;
                case 3:
                    version = new Version(
                        numberComponents[0],
                        numberComponents[1],
                        numberComponents[2]);
                    return true;
                default:
                    version = new Version(
                        numberComponents[0],
                        numberComponents[1],
                        numberComponents[2],
                        numberComponents[3]);
                    return true;
            }
        }
    }
}
