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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

namespace RelaxVersioner;

[NonParallelizable]
public sealed class PackTargetsTests
{
    private static readonly SemaphoreSlim tasksAssemblyLock = new(1, 1);
    private static readonly SemaphoreSlim toolingAssemblyLock = new(1, 1);
    private static string? tasksAssemblyPath;
    private static string? toolingAssemblyPath;

    [Test]
    public async Task RelaxVersionerBeforeCompile_SelectiveMode_UsesGlobalPropertiesWithoutLegacyDump()
    {
#if !NET8_0
        Assert.Ignore("This integration test runs once on net8.0.");
#else
        var repositoryRoot = GetRepositoryRoot();
        var builtTasksAssemblyPath = await EnsureTasksAssemblyPathAsync(repositoryRoot);
        var builtToolingAssemblyPath = await EnsureToolingAssemblyPathAsync(repositoryRoot);
        var builtToolingDirectoryPath = Path.GetDirectoryName(builtToolingAssemblyPath)!;
        var tempPath = Path.Combine(
            Path.GetTempPath(),
            $"RelaxVersionerSelectiveTargetsTest_{Guid.NewGuid():N}");

        try
        {
            Directory.CreateDirectory(tempPath);
            await InitializeProjectRepositoryAsync(tempPath);
            await File.WriteAllTextAsync(
                Path.Combine(tempPath, "RelaxVersioner.rules"),
                """
                <RelaxVersioner>
                  <WriterRules>
                    <Language>C#</Language>
                    <Import>System.Reflection</Import>
                    <Rule name="AssemblyMetadata" key="RequestMarker">{RequestMarker}</Rule>
                  </WriterRules>
                </RelaxVersioner>
                """);

            var projectPath = Path.Combine(tempPath, "SelectiveProbe.csproj");
            await File.WriteAllTextAsync(
                projectPath,
                $$"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <Import Project="{{EscapePath(Path.Combine(repositoryRoot, "RelaxVersioner", "build", "RelaxVersioner.props"))}}" />

                  <PropertyGroup>
                    <TargetFramework>net8.0</TargetFramework>
                    <_RVB_MSBuildTaskPath>{{EscapePath(builtTasksAssemblyPath)}}</_RVB_MSBuildTaskPath>
                    <RelaxVersionerToolingRuntimeName>dotnet </RelaxVersionerToolingRuntimeName>
                    <RelaxVersionerToolingDir>{{EscapePath(builtToolingDirectoryPath)}}</RelaxVersionerToolingDir>
                    <RelaxVersionerToolingPath>{{EscapePath(builtToolingAssemblyPath)}}</RelaxVersionerToolingPath>
                    <RelaxVersionerPropertyCollectionMode>Selective</RelaxVersionerPropertyCollectionMode>
                    <RelaxVersionerCheckWorkingDirectoryStatus>false</RelaxVersionerCheckWorkingDirectoryStatus>
                  </PropertyGroup>

                  <Import Project="{{EscapePath(Path.Combine(repositoryRoot, "RelaxVersioner", "build", "RelaxVersioner.targets"))}}" />
                </Project>
                """);

            await TestUtilities.RunCommandAsync(
                "dotnet",
                tempPath,
                $"build \"{projectPath}\" -p:RequestMarker=alpha -nr:false");

            var generatedPath = Path.Combine(
                tempPath,
                "obj",
                "Debug",
                "net8.0",
                "RelaxVersioner_Metadata.cs");
            var propertiesPath = Path.Combine(
                tempPath,
                "obj",
                "Debug",
                "net8.0",
                "RelaxVersioner_Properties.xml");
            var generatedContent = await File.ReadAllTextAsync(generatedPath);

            Assert.Multiple(() =>
            {
                Assert.That(File.Exists(generatedPath), Is.True);
                Assert.That(File.Exists(propertiesPath), Is.False,
                    "Selective mode should not produce the legacy properties dump.");
                Assert.That(generatedContent, Does.Contain("RequestMarker"));
                Assert.That(generatedContent, Does.Contain("alpha"));
            });
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

    [Test]
    public async Task RelaxVersionerBeforeCompile_CompareMode_PreservesLegacyDump()
    {
#if !NET8_0
        Assert.Ignore("This integration test runs once on net8.0.");
#else
        var repositoryRoot = GetRepositoryRoot();
        var builtTasksAssemblyPath = await EnsureTasksAssemblyPathAsync(repositoryRoot);
        var builtToolingAssemblyPath = await EnsureToolingAssemblyPathAsync(repositoryRoot);
        var builtToolingDirectoryPath = Path.GetDirectoryName(builtToolingAssemblyPath)!;
        var tempPath = Path.Combine(
            Path.GetTempPath(),
            $"RelaxVersionerCompareTargetsTest_{Guid.NewGuid():N}");

        try
        {
            Directory.CreateDirectory(tempPath);
            await InitializeProjectRepositoryAsync(tempPath);
            await File.WriteAllTextAsync(
                Path.Combine(tempPath, "RelaxVersioner.rules"),
                """
                <RelaxVersioner>
                  <WriterRules>
                    <Language>C#</Language>
                    <Import>System.Reflection</Import>
                    <Rule name="AssemblyMetadata" key="RequestMarker">{RequestMarker}</Rule>
                  </WriterRules>
                </RelaxVersioner>
                """);

            var projectPath = Path.Combine(tempPath, "CompareProbe.csproj");
            await File.WriteAllTextAsync(
                projectPath,
                $$"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <Import Project="{{EscapePath(Path.Combine(repositoryRoot, "RelaxVersioner", "build", "RelaxVersioner.props"))}}" />

                  <PropertyGroup>
                    <TargetFramework>net8.0</TargetFramework>
                    <_RVB_MSBuildTaskPath>{{EscapePath(builtTasksAssemblyPath)}}</_RVB_MSBuildTaskPath>
                    <RelaxVersionerToolingRuntimeName>dotnet </RelaxVersionerToolingRuntimeName>
                    <RelaxVersionerToolingDir>{{EscapePath(builtToolingDirectoryPath)}}</RelaxVersionerToolingDir>
                    <RelaxVersionerToolingPath>{{EscapePath(builtToolingAssemblyPath)}}</RelaxVersionerToolingPath>
                    <RelaxVersionerPropertyCollectionMode>Compare</RelaxVersionerPropertyCollectionMode>
                    <RelaxVersionerCheckWorkingDirectoryStatus>false</RelaxVersionerCheckWorkingDirectoryStatus>
                  </PropertyGroup>

                  <Import Project="{{EscapePath(Path.Combine(repositoryRoot, "RelaxVersioner", "build", "RelaxVersioner.targets"))}}" />
                </Project>
                """);

            await TestUtilities.RunCommandAsync(
                "dotnet",
                tempPath,
                $"build \"{projectPath}\" -p:RequestMarker=beta -nr:false");

            var generatedPath = Path.Combine(
                tempPath,
                "obj",
                "Debug",
                "net8.0",
                "RelaxVersioner_Metadata.cs");
            var propertiesPath = Path.Combine(
                tempPath,
                "obj",
                "Debug",
                "net8.0",
                "RelaxVersioner_Properties.xml");
            var generatedContent = await File.ReadAllTextAsync(generatedPath);

            Assert.Multiple(() =>
            {
                Assert.That(File.Exists(generatedPath), Is.True);
                Assert.That(File.Exists(propertiesPath), Is.True,
                    "Compare mode should keep the legacy properties dump for actual formatting.");
                Assert.That(generatedContent, Does.Contain("RequestMarker"));
                Assert.That(generatedContent, Does.Contain("beta"));
            });
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

    [Test]
    public async Task RelaxVersionerPackPrepare_UsesUniqueWorkspacePerBuildRequest()
    {
#if !NET8_0
        Assert.Ignore("This integration test runs once on net8.0.");
#else

        var repositoryRoot = GetRepositoryRoot();
        var builtTasksAssemblyPath = await EnsureTasksAssemblyPathAsync(repositoryRoot);
        var tempPath = Path.Combine(
            Path.GetTempPath(),
            $"RelaxVersionerPackTargetsTest_{Guid.NewGuid():N}");

        try
        {
            Directory.CreateDirectory(tempPath);

            var projectPath = Path.Combine(tempPath, "PackWorkspaceProbe.csproj");
            await File.WriteAllTextAsync(
                projectPath,
                $$"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <Import Project="{{EscapePath(Path.Combine(repositoryRoot, "RelaxVersioner", "build", "RelaxVersioner.props"))}}" />

                  <PropertyGroup>
                    <TargetFramework>net8.0</TargetFramework>
                    <IsPackable>true</IsPackable>
                    <_RVB_MSBuildTaskPath>{{EscapePath(builtTasksAssemblyPath)}}</_RVB_MSBuildTaskPath>
                  </PropertyGroup>

                  <Import Project="{{EscapePath(Path.Combine(repositoryRoot, "RelaxVersioner", "build", "RelaxVersioner.targets"))}}" />

                  <Target Name="CapturePackWorkspace" DependsOnTargets="RelaxVersionerPackPrepare">
                    <WriteLinesToFile
                      File="$(BaseIntermediateOutputPath)$(RequestMarker).log"
                      Lines="$(RelaxVersionerPropertiesPath)"
                      Overwrite="true" />
                  </Target>

                  <Target Name="RunParallelPackLikeRequests">
                    <ItemGroup>
                      <_PackRequests Include="$(MSBuildProjectFullPath)" AdditionalProperties="RequestMarker=first" />
                      <_PackRequests Include="$(MSBuildProjectFullPath)" AdditionalProperties="RequestMarker=second" />
                    </ItemGroup>

                    <MSBuild
                      Projects="@(_PackRequests)"
                      Targets="CapturePackWorkspace"
                      BuildInParallel="true" />
                  </Target>
                </Project>
                """);

            await TestUtilities.RunCommandAsync(
                "dotnet",
                tempPath,
                $"msbuild \"{projectPath}\" -t:RunParallelPackLikeRequests -m:2 -nr:false");

            var firstPath = (await File.ReadAllTextAsync(Path.Combine(tempPath, "obj", "first.log"))).Trim();
            var secondPath = (await File.ReadAllTextAsync(Path.Combine(tempPath, "obj", "second.log"))).Trim();
            var legacyPackPath = Path.GetFullPath(
                Path.Combine(tempPath, "obj", "Debug", "RelaxVersioner_Properties.xml"));

            Assert.Multiple(() =>
            {
                Assert.That(firstPath, Is.Not.Empty);
                Assert.That(secondPath, Is.Not.Empty);
                Assert.That(firstPath, Is.Not.EqualTo(secondPath),
                    "Pack-like requests should not share the same properties path.");
                Assert.That(firstPath, Does.Contain("RelaxVersioner_Pack"));
                Assert.That(secondPath, Does.Contain("RelaxVersioner_Pack"));
                Assert.That(firstPath, Does.EndWith("RelaxVersioner_Properties.xml"));
                Assert.That(secondPath, Does.EndWith("RelaxVersioner_Properties.xml"));
                Assert.That(Path.GetFullPath(firstPath), Is.Not.EqualTo(legacyPackPath));
                Assert.That(Path.GetFullPath(secondPath), Is.Not.EqualTo(legacyPackPath));
                Assert.That(Path.GetDirectoryName(firstPath), Is.Not.EqualTo(Path.GetDirectoryName(secondPath)),
                    "Each request should resolve a different workspace directory.");
            });
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

    private static async Task<string> EnsureTasksAssemblyPathAsync(string repositoryRoot)
    {
        await tasksAssemblyLock.WaitAsync();
        try
        {
            if (!string.IsNullOrWhiteSpace(tasksAssemblyPath) &&
                File.Exists(tasksAssemblyPath))
            {
                return tasksAssemblyPath;
            }

            var projectPath = Path.Combine(
                repositoryRoot,
                "RelaxVersioner.Tasks",
                "RelaxVersioner.Tasks.csproj");

            await TestUtilities.RunCommandAsync(
                "dotnet",
                repositoryRoot,
                $"build \"{projectPath}\" -c Debug -f netstandard2.0 -p:BOOTSTRAP=True -nr:false");

            tasksAssemblyPath = Path.Combine(
                repositoryRoot,
                "RelaxVersioner.Tasks",
                "bin",
                "Debug",
                "netstandard2.0",
                "RelaxVersioner.Tasks.dll");

            Assert.That(File.Exists(tasksAssemblyPath), Is.True,
                $"Task assembly should exist: {tasksAssemblyPath}");
            return tasksAssemblyPath;
        }
        finally
        {
            tasksAssemblyLock.Release();
        }
    }

    private static async Task<string> EnsureToolingAssemblyPathAsync(string repositoryRoot)
    {
        await toolingAssemblyLock.WaitAsync();
        try
        {
            if (!string.IsNullOrWhiteSpace(toolingAssemblyPath) &&
                File.Exists(toolingAssemblyPath))
            {
                return toolingAssemblyPath;
            }

            var projectPath = Path.Combine(
                repositoryRoot,
                "RelaxVersioner",
                "RelaxVersioner.csproj");

            await TestUtilities.RunCommandAsync(
                "dotnet",
                repositoryRoot,
                $"build \"{projectPath}\" -c Debug -f net8.0 -p:BOOTSTRAP=True -nr:false");

            toolingAssemblyPath = Path.Combine(
                repositoryRoot,
                "RelaxVersioner",
                "bin",
                "Debug",
                "net8.0",
                "rv.dll");

            Assert.That(File.Exists(toolingAssemblyPath), Is.True,
                $"Tooling assembly should exist: {toolingAssemblyPath}");
            return toolingAssemblyPath;
        }
        finally
        {
            toolingAssemblyLock.Release();
        }
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);

        while (directory != null)
        {
            if (directory.EnumerateFiles("RelaxVersioner.sln", SearchOption.TopDirectoryOnly).Any())
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Repository root could not be located.");
    }

    private static async Task InitializeProjectRepositoryAsync(string tempPath)
    {
        await TestUtilities.InitializeGitRepositoryWithMainBranch(tempPath);
        await TestUtilities.RunGitCommandAsync(tempPath, "config user.email \"test@example.com\"");
        await TestUtilities.RunGitCommandAsync(tempPath, "config user.name \"Test User\"");
        await File.WriteAllTextAsync(
            Path.Combine(tempPath, "Program.cs"),
            """
            public static class Program
            {
                public static void Main()
                {
                }
            }
            """);
        await TestUtilities.RunGitCommandAsync(tempPath, "add .");
        await TestUtilities.RunGitCommandAsync(tempPath, "commit -m \"Initial commit\"");
    }

    private static string EscapePath(string path) =>
        path.Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;");
}
