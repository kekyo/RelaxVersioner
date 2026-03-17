////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

namespace RelaxVersioner;

[Parallelizable(ParallelScope.All)]
public sealed class PropertyCollectorTests
{
    [Test]
    public void CollectRequestedMsBuildPropertyNames_SourceCodeRules_UsesRootKeysAndFiltersBuiltIns()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerPropertyCollector_{Guid.NewGuid():N}");

        try
        {
            Directory.CreateDirectory(tempPath);

            var projectPath = Path.Combine(tempPath, "Sample.csproj");
            File.WriteAllText(projectPath, "<Project />");
            File.WriteAllText(
                Path.Combine(tempPath, "RelaxVersioner.rules"),
                """
                <RelaxVersioner>
                  <WriterRules>
                    <Language>C#</Language>
                    <Rule name="AssemblyMetadata" key="Configuration">{Configuration}</Rule>
                    <Rule name="AssemblyMetadata" key="Path">{PATH}</Rule>
                    <Rule name="AssemblyMetadata" key="Version">{versionLabel}</Rule>
                    <Rule name="AssemblyMetadata" key="Subject">{commit.Subject}</Rule>
                    <Rule name="AssemblyMetadata" key="Configuration2">{Configuration}</Rule>
                  </WriterRules>
                </RelaxVersioner>
                """);

            var context = new ProcessorContext
            {
                ProjectDirectory = projectPath,
                ProjectPath = projectPath,
                Language = "C#",
            };

            var propertyNames = PropertyCollector.CollectRequestedMsBuildPropertyNames(context);

            Assert.That(propertyNames, Is.EqualTo(new[] { "Configuration", "PATH" }));
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
    }

    [Test]
    public void CollectRequestedMsBuildPropertyNames_Replace_UsesBufferedInputText()
    {
        var context = new ProcessorContext
        {
            ProjectDirectory = Directory.GetCurrentDirectory(),
            Language = "Replace",
            ReplaceInputText = "prefix {Configuration} {commit.Subject} suffix {Configuration}",
        };

        var propertyNames = PropertyCollector.CollectRequestedMsBuildPropertyNames(context);

        Assert.That(propertyNames, Is.EqualTo(new[] { "Configuration" }));
    }

    [Test]
    public async Task LoadAsync_SelectiveMode_UsesResolvedProjectAndGlobalProperties()
    {
#if !NET8_0
        Assert.Ignore("This integration-style test runs once on net8.0.");
#else
        var tempPath = Path.Combine(Path.GetTempPath(), $"RelaxVersionerSelectiveCollector_{Guid.NewGuid():N}");

        try
        {
            Directory.CreateDirectory(tempPath);

            var projectPath = Path.Combine(tempPath, "Sample.proj");
            await File.WriteAllTextAsync(
                projectPath,
                """
                <Project>
                  <PropertyGroup>
                    <Configuration Condition="'$(Configuration)' == ''">DefaultConfiguration</Configuration>
                  </PropertyGroup>
                </Project>
                """);

            var globalPropertiesPath = Path.Combine(tempPath, "global-properties.xml");
            await File.WriteAllTextAsync(
                globalPropertiesPath,
                """
                <Properties>
                  <Property Name="Configuration">SelectiveConfiguration</Property>
                </Properties>
                """);

            using var outWriter = new StringWriter();
            using var errorWriter = new StringWriter();
            var logger = Logger.Create("PropertyCollectorTest", LogImportance.Normal, outWriter, errorWriter, errorWriter);

            var context = new ProcessorContext
            {
                ProjectDirectory = projectPath,
                ProjectPath = projectPath,
                Language = "Text",
                OutputPath = Path.Combine(tempPath, "result.txt"),
                TextFormat = "{Configuration}",
                PropertyCollectionMode = PropertyCollectionMode.Selective,
                GlobalPropertiesPath = globalPropertiesPath,
                MsBuildRuntimeType = "Core",
            };

            var properties = await PropertyCollector.LoadAsync(logger, context, default);

            Assert.That(properties.TryGetValue("Configuration", out var configuration), Is.True);
            Assert.That(configuration, Is.EqualTo("SelectiveConfiguration"));
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                try { Directory.Delete(tempPath, true); } catch { }
            }
        }
#endif
    }
}
