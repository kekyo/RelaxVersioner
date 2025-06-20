////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using NUnit.Framework;

namespace RelaxVersioner;

[Parallelizable(ParallelScope.All)]
public sealed class VersionTests
{
    [TestCase("1.2.3", true, "1.2.3")]
    [TestCase("v1.2.3", true, "1.2.3")]
    [TestCase("1.2", true, "1.2")]
    [TestCase("1", true, "1.0")]
    [TestCase("1.2.3.4", true, "1.2.3.4")]
    [TestCase("0.0.1", true, "0.0.1")]
    [TestCase("65535.65535.65535.65535", true, "65535.65535.65535.65535")]
    public void TryParse_ValidVersions_ShouldSucceed(string input, bool expectedSuccess, string expectedString)
    {
        var result = Version.TryParse(input, out var version);
        
        Assert.That(result, Is.EqualTo(expectedSuccess));
        if (expectedSuccess)
        {
            Assert.That(version.ToString(), Is.EqualTo(expectedString));
        }
    }

    [TestCase("20250632")]
    [TestCase("65536.0.0")]
    [TestCase("0.65536.0")]
    [TestCase("0.0.65536")]
    [TestCase("1.2.3.65536")]
    [TestCase("100000.1.2")]
    [TestCase("v20250632")]
    [TestCase("20250632.0")]
    public void TryParse_LargeNumbers_ShouldFail(string input)
    {
        var result = Version.TryParse(input, out var version);
        
        Assert.That(result, Is.False, $"Expected parsing of '{input}' to fail due to 16-bit limitations");
    }

    [TestCase("1.2.3.4.5")]
    [TestCase("1.2.3.4.5.6")]
    [TestCase("v1.2.3.4.5")]
    [TestCase("0.1.2.3.4")]
    public void TryParse_TooManyComponents_ShouldFail(string input)
    {
        var result = Version.TryParse(input, out var version);
        
        Assert.That(result, Is.False, $"Expected parsing of '{input}' to fail due to too many components (>4)");
    }

    [TestCase("")]
    [TestCase("abc")]
    [TestCase("1.abc")]
    [TestCase("1.2.abc")]
    [TestCase("...")]
    [TestCase("-1.2.3")]
    public void TryParse_InvalidVersions_ShouldFail(string input)
    {
        var result = Version.TryParse(input, out var version);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void ComponentCount_ShouldReturnCorrectValue()
    {
        Assert.That(new Version(1).ComponentCount, Is.EqualTo(1));
        Assert.That(new Version(1, 2).ComponentCount, Is.EqualTo(2));
        Assert.That(new Version(1, 2, 3).ComponentCount, Is.EqualTo(3));
        Assert.That(new Version(1, 2, 3, 4).ComponentCount, Is.EqualTo(4));
    }

    [Test]
    public void ToString_ShouldFormatCorrectly()
    {
        Assert.That(new Version(1).ToString(), Is.EqualTo("1.0"));
        Assert.That(new Version(1, 2).ToString(), Is.EqualTo("1.2"));
        Assert.That(new Version(1, 2, 3).ToString(), Is.EqualTo("1.2.3"));
        Assert.That(new Version(1, 2, 3, 4).ToString(), Is.EqualTo("1.2.3.4"));
    }

    [Test]
    public void CompareTo_ShouldWorkCorrectly()
    {
        var version1 = new Version(1, 2, 3);
        var version2 = new Version(1, 2, 4);
        var version3 = new Version(1, 2, 3);

        Assert.That(version1.CompareTo(version2), Is.LessThan(0));
        Assert.That(version2.CompareTo(version1), Is.GreaterThan(0));
        Assert.That(version1.CompareTo(version3), Is.EqualTo(0));
    }
} 