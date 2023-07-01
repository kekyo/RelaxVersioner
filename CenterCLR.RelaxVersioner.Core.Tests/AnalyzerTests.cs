////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version information inserter.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using GitReader;
using GitReader.Structures;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace RelaxVersioner;

public sealed class AnalyzerTests
{
    [TestCase("NothingAnyTags1", "0.0.4")]
    [TestCase("NothingAnyTags2", "0.0.5")]
    [TestCase("NothingAnyTags3", "0.0.5")]
    [TestCase("NothingAnyTags4", "0.0.6")]
    [TestCase("TagInTop", "0.15.2")]
    [TestCase("TagInPrimary", "0.10.1")]
    [TestCase("TagInSecondary", "0.20.1")]
    [TestCase("TagInBoth", "0.10.5")]
    [TestCase("MultiRoot1", "0.10.6")]
    [TestCase("MultiRoot2", "0.10.22")]
    public async Task LookupVersionLabel(string repositoryName, string expectedString)
    {
        using var repository = await Repository.Factory.OpenStructureAsync(
            Path.Combine(TestsSetUp.BasePath, repositoryName));

        var actual = await Analyzer.LookupVersionLabelAsync(repository.Head!, default);

        Assert.AreEqual(expectedString, actual.ToString());
    }
}
