////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace RelaxVersioner;

public sealed class Version :
    IEquatable<Version>, IComparable<Version>
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

    public override int GetHashCode() =>
        this.Major.GetHashCode() ^
        this.Minor.GetHashCode() ^
        this.Build.GetHashCode() ^
        this.Revision.GetHashCode();

    public bool Equals(Version? rhs) =>
        rhs is { } &&
        this.Major.Equals(rhs.Major) &&
        this.Minor.Equals(rhs.Minor) &&
        this.Build.Equals(rhs.Build) &&
        this.Revision.Equals(rhs.Revision);

    public override bool Equals(object? obj) =>
        obj is Version rhs && this.Equals(rhs);

    public int CompareTo(Version? rhs)
    {
        var major = this.Major.CompareTo(rhs!.Major);
        if (major != 0)
        {
            return major;
        }

        static int Compare(int? lhs, int? rhs)
        {
            var l = lhs is { } lv ? lv : 0;
            var r = rhs is { } rv ? rv : 0;
            return l.CompareTo(r);
        }

        var minor = Compare(this.Minor, rhs.Minor);
        if (minor != 0)
        {
            return minor;
        }
        var build = Compare(this.Build, rhs.Build);
        if (build != 0)
        {
            return build;
        }
        var revision = Compare(this.Revision, rhs.Revision);
        return revision;
    }

    private IEnumerable<int> GetComponents()
    {
        yield return this.Major;

        // HACK: The version number format is valid on 2 or more components.
        yield return this.Minor.HasValue ? this.Minor.Value : 0;

        if (this.Build.HasValue)
        {
            yield return this.Build.Value;
            if (this.Revision.HasValue)
            {
                yield return this.Revision.Value;
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
                version = null!;
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
