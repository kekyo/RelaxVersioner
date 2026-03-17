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
    private static string? tasksAssemblyPath;

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

    private static string EscapePath(string path) =>
        path.Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;");
}
