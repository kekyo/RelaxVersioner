# RelaxVersioner
![RelaxVersioner](Images/CenterCLR.RelaxVersioner.128.png)

[![Japanese language](Images/Japanese.256.png)](https://github.com/kekyo/CenterCLR.RelaxVersioner/blob/master/README.ja.md)

# Status

[![Project Status: Active – The project has reached a stable, usable state and is being actively developed.](https://www.repostatus.org/badges/latest/active.svg)](https://www.repostatus.org/#active)

| | master | devel |
|:---|:---|:---|
| Packages | [![NuGet RelaxVersioner (master)](https://img.shields.io/nuget/v/RelaxVersioner.svg?style=flat)](https://www.nuget.org/packages/RelaxVersioner) | [![MyGet RelaxVersioner (devel)](https://img.shields.io/myget/kekyo/v/RelaxVersioner.svg?style=flat&label=myget)](https://www.myget.org/feed/kekyo/package/nuget/RelaxVersioner)
| Continuous integration | [![RelaxVersioner CI build (master)](https://github.com/kekyo/CenterCLR.RelaxVersioner/workflows/.NET/badge.svg?branch=master)](https://github.com/kekyo/CenterCLR.RelaxVersioner/actions) | [![RelaxVersioner CI build (devel)](https://github.com/kekyo/CenterCLR.RelaxVersioner/workflows/.NET/badge.svg?branch=devel)](https://github.com/kekyo/CenterCLR.RelaxVersioner/actions)

## What is this?

* RelaxVersioner is a easy, full-automatic, git based version inserter for .NET 5/.NET Core/.NET Framework.
* If you use RelaxVersioner, version handling ONLY use with Git tags/branches/commit messages. Of course you don't need more tooling usage, and easy integrates continuous-integration environments.
* Target language/environments (Probably fits most current .NET development environments):
  * C#, F#, VB.NET, C++/CLI and NuGet packaging (dotnet cli packer).
  * Visual Studio 2019/2017/2015, Rider, dotnet SDK cli, MSBuild on netcoreapp2.1/net461 environment (NOT your project platform) and related IDEs.
  * Linux(x64) and Windows(x86/x64).  (The project validates only them, but maybe can run at same as [libgit2sharp](https://github.com/libgit2/libgit2sharp) required environment)
* Auto collect version information from local Git repository tags/branch name.
* Independent AssemblyInfo.cs file, generated code will output into a temporary file. (Not manipulate directly AssemblyInfo.cs file).
* Place source code location which isn't obstructive for Git. (ex: obj/Debug)
* You can customize output attribute/values with custom rule set file.

### Result for assembly property at the explorer

![Result for assembly property at the explorer](Images/Explorer.png)

### Result for assembly wide attributes at ILSpy

![Assembly wide attributes at ILSpy](Images/ILSpy.png)

----

## Sample output code

### For C#:

``` csharp
using System.Reflection;
[assembly: AssemblyVersion("1.0.21")]
[assembly: AssemblyFileVersion("2020.12.20.33529")]
[assembly: AssemblyInformationalVersion("1.0.21-561387e2f6dc90046f56ef4c3ac501aad0d5ec0a")]
[assembly: AssemblyMetadata("Date","Sun, 20 Dec 2020 09:37:39 GMT")]
[assembly: AssemblyMetadata("Branch","master")]
[assembly: AssemblyMetadata("Tags","")]
[assembly: AssemblyMetadata("Author","Kouji Matsui <k@kekyo.net>")]
[assembly: AssemblyMetadata("Committer","Kouji Matsui <k@kekyo.net>")]
[assembly: AssemblyMetadata("Message","Merge branch 'devel'")]
[assembly: AssemblyMetadata("Build","")]
[assembly: AssemblyMetadata("Generated","Sun, 20 Dec 2020 09:37:43 GMT")]
[assembly: AssemblyMetadata("Platform","AnyCPU")]
[assembly: AssemblyMetadata("BuildOn","Unix")]
[assembly: AssemblyMetadata("SdkVersion","5.0.101")]

namespace YourApp
{
  internal static class ThisAssembly
  {
    public const string AssemblyVersion = "1.0.21";
    public const string AssemblyFileVersion = "2020.12.20.33529";
    public const string AssemblyInformationalVersion = "1.0.21-561387e2f6dc90046f56ef4c3ac501aad0d5ec0a";
    public static class AssemblyMetadata
    {
      public const string Date = "Sun, 20 Dec 2020 09:37:39 GMT";
      public const string Branch = "master";
      public const string Tags = "";
      public const string Author = "Kouji Matsui <k@kekyo.net>";
      public const string Committer = "Kouji Matsui <k@kekyo.net>";
      public const string Message = "Merge branch 'devel'";
      public const string Build = "";
      public const string Generated = "Sun, 20 Dec 2020 09:37:43 GMT";
      public const string Platform = "AnyCPU";
      public const string BuildOn = "Unix";
      public const string SdkVersion = "5.0.101";
    }
  }
}
```

### For F#:

``` fsharp
namespace global
  open System.Reflection
  [<assembly: AssemblyVersion("1.0.21")>]
  [<assembly: AssemblyFileVersion("2020.12.20.33529")>]
  [<assembly: AssemblyInformationalVersion("1.0.21-561387e2f6dc90046f56ef4c3ac501aad0d5ec0a")>]
  [<assembly: AssemblyMetadata("Date","Sun, 20 Dec 2020 09:37:39 GMT")>]
  [<assembly: AssemblyMetadata("Branch","master")>]
  [<assembly: AssemblyMetadata("Tags","")>]
  [<assembly: AssemblyMetadata("Author","Kouji Matsui <k@kekyo.net>")>]
  [<assembly: AssemblyMetadata("Committer","Kouji Matsui <k@kekyo.net>")>]
  [<assembly: AssemblyMetadata("Message","Merge branch 'devel'")>]
  [<assembly: AssemblyMetadata("Build","")>]
  [<assembly: AssemblyMetadata("Generated","Sun, 20 Dec 2020 09:38:33 GMT")>]
  [<assembly: AssemblyMetadata("Platform","AnyCPU")>]
  [<assembly: AssemblyMetadata("BuildOn","Unix")>]
  [<assembly: AssemblyMetadata("SdkVersion","5.0.101")>]
  do()

namespace global
  module internal ThisAssembly =
    [<Literal>]
    let AssemblyVersion = "1.0.21";
    [<Literal>]
    let AssemblyFileVersion = "2020.12.20.33529";
    [<Literal>]
    let AssemblyInformationalVersion = "1.0.21-561387e2f6dc90046f56ef4c3ac501aad0d5ec0a";
    module AssemblyMetadata =
      [<Literal>]
      let Date = "Sun, 20 Dec 2020 09:37:39 GMT";
      [<Literal>]
      let Branch = "master";
      [<Literal>]
      let Tags = "";
      [<Literal>]
      let Author = "Kouji Matsui <k@kekyo.net>";
      [<Literal>]
      let Committer = "Kouji Matsui <k@kekyo.net>";
      [<Literal>]
      let Message = "Merge branch 'devel'";
      [<Literal>]
      let Build = "";
      [<Literal>]
      let Generated = "Sun, 20 Dec 2020 09:38:33 GMT";
      [<Literal>]
      let Platform = "AnyCPU";
      [<Literal>]
      let BuildOn = "Unix";
      [<Literal>]
      let SdkVersion = "5.0.101";
  do()
```

### For VB.NET:

``` visualbasic
<Assembly: AssemblyVersion("1.0.21")>
<Assembly: AssemblyFileVersion("2020.12.20.33529")>
<Assembly: AssemblyInformationalVersion("1.0.21-561387e2f6dc90046f56ef4c3ac501aad0d5ec0a")>
<Assembly: AssemblyMetadata("Date","Sun, 20 Dec 2020 09:37:39 GMT")>
<Assembly: AssemblyMetadata("Branch","master")>
<Assembly: AssemblyMetadata("Tags","")>
<Assembly: AssemblyMetadata("Author","Kouji Matsui <k@kekyo.net>")>
<Assembly: AssemblyMetadata("Committer","Kouji Matsui <k@kekyo.net>")>
<Assembly: AssemblyMetadata("Message","Merge branch 'devel'")>
<Assembly: AssemblyMetadata("Build","")>
<Assembly: AssemblyMetadata("Generated","Sun, 20 Dec 2020 09:38:33 GMT")>
<Assembly: AssemblyMetadata("TargetFramework","")>
<Assembly: AssemblyMetadata("Platform","x64")>
<Assembly: AssemblyMetadata("BuildOn","Windows_NT")>
<Assembly: AssemblyMetadata("SdkVersion","5.0.101")>

Namespace global.YourApp
  Module ThisAssembly
    Public Const AssemblyVersion As String = "1.0.21"
    Public Const AssemblyFileVersion As String = "2020.12.20.33529"
    Public Const AssemblyInformationalVersion As String = "1.0.21-561387e2f6dc90046f56ef4c3ac501aad0d5ec0a"
    Public NotInheritable Class AssemblyMetadata
      Public Const Date As String = "Sun, 20 Dec 2020 09:37:39 GMT"
      Public Const Branch As String = "master"
      Public Const Tags As String = ""
      Public Const Author As String = "Kouji Matsui <k@kekyo.net>"
      Public Const Committer As String = "Kouji Matsui <k@kekyo.net>"
      Public Const Message As String = "Merge branch 'devel'"
      Public Const Build As String = ""
      Public Const Generated As String = "Sun, 20 Dec 2020 09:38:33 GMT"
      Public Const Platform As String = "AnyCPU"
      Public Const BuildOn As String = "Windows_NT"
      Public Const SdkVersion As String = "5.0.101"
    End Class
  End Module
End Namespace
```

### For C++/CLI:

``` cpp
using namespace System::Reflection;
[assembly: AssemblyVersion("1.0.44")];
[assembly: AssemblyFileVersion("2020.12.20.33300")];
[assembly: AssemblyInformationalVersion("1.0.21-7faf4071fdc2f169ecc58d705ea3304dd91af414")];
[assembly: AssemblyMetadata("Date","Sun, 20 Dec 2020 09:30:00 GMT")];
[assembly: AssemblyMetadata("Branch","devel")];
[assembly: AssemblyMetadata("Tags","")];
[assembly: AssemblyMetadata("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyMetadata("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyMetadata("Message","Fixed generating path in C++.")];
[assembly: AssemblyMetadata("Build","")];
[assembly: AssemblyMetadata("Generated","Sun, 20 Dec 2020 09:34:03 GMT")];
[assembly: AssemblyMetadata("Platform","x64")];
[assembly: AssemblyMetadata("BuildOn","Windows_NT")];
[assembly: AssemblyMetadata("SdkVersion","5.0.101")];

private ref class ThisAssembly abstract sealed
{
public:
    literal System::String^ AssemblyVersion = "1.0.44";
    literal System::String^ AssemblyFileVersion = "2020.12.20.33300";
    literal System::String^ AssemblyInformationalVersion = "1.0.21-7faf4071fdc2f169ecc58d705ea3304dd91af414";
    ref class AssemblyMetadata abstract sealed
    {
        literal System::String^ Date = "Sun, 20 Dec 2020 09:30:00 GMT";
        literal System::String^ Branch = "devel";
        literal System::String^ Tags = "";
        literal System::String^ Author = "Kouji Matsui <k@kekyo.net>";
        literal System::String^ Committer = "Kouji Matsui <k@kekyo.net>";
        literal System::String^ Message = "Fixed generating path in C++.";
        literal System::String^ Build = "";
        literal System::String^ Generated = "Sun, 20 Dec 2020 09:34:03 GMT";
        literal System::String^ Platform = "x64";
        literal System::String^ BuildOn = "Windows_NT";
        literal System::String^ SdkVersion = "5.0.101";
    };
};
```

----

## Getting started

### Start guide

[Refer start guides.](./STARTGUIDE.md)

### How to use

* Search NuGet repository: ["RelaxVersioner"](https://www.nuget.org/packages/RelaxVersioner/), and install.
* (Optional if you're using old MSBuild project): Before build, comment out "AssemblyVersion" and "AssemblyFileVersion" attribute in AssemblyInfo.cs default definitions (will cause build error by duplicate definition). If you use custom rule set, continue use this definitions.
* After installed, build project. Auto-apply version informations into assembly attributes. Some attributes are looking for [ILSpy](https://github.com/icsharpcode/ILSpy) or Windows Explorer property page.
* You can use custom rule set file naming "RelaxVersioner.rules" into project folder "$(ProjectDir)" or solution folder "$(SolutionDir)". Current no documentation custom rule set file, see also below.

### Example for development cycles

1. You create new C#,F# or another project.
2. Install "RelaxVersioner" from NuGet.
3. (Optional): Comment outs default "AssemblyVersion" and "AssemblyFileVersion" declarations in AssemblyInfo.cs.
4. Let's build now! Output binary applied version informations.
  * Default declaration of AssemblyVersion="0.0.1.0", AssemblyFileVersion="(Build date on 2sec prec.)" (ex:"2016.05.12.11523"）
  * Applied AssemblyVersionMetadata from local Git repository (Author, Branch, Tags）. But this example git repository not created , so there declarations containing "Unknown".
5. Create Git local repository (command: git init). And commit with message.
6. Build retry. Output binary applied Author, Branch, Tags now.
7. You are tagging current commit. For example "0.5.4" or "v0.5.4". Rebuild, then contains AssemblyVersion is "0.5.4" now.
  * The auto increment feature: If your current commit doesn't apply any tags, RelaxVersioner will traverse committing history and auto increment commit depth for tail of version component. For example, tagged "0.5.4' at 2 older commit. The auto incrementer will calculate version "0.5.6".
  * RelaxVersioner will traverse first priority for primary parent and then next others. Therefore, if you're using branch strategy, you can apply auto increment  with different version each branch when tagged different version each branch at bottom commit. For example: The tick-tock model for the master branch tagged "1.0.0" and devel branch tagged "1.1.0".
8. All rights codes, tags. Push to remote repository, all done.
9. Development cycles: next codes change and ready to release, you are tagging new version and then build, output binary auto tagged in AssemblyVersion and store informations.
  * We can apply with automated version number when "dotnet cli" for generate NuGet package (`PackageVersion` and `PackageReleaseNotes` attributes). You can use only `dotnet pack` command.

----

## Sample custom rule set file (RelaxVersioner.rules):

``` xml
<?xml version="1.0" encoding="utf-8"?>
<RelaxVersioner version="1.0">
  <WriterRules>
    <!-- Target languages -->
    <Language>C#</Language>
    <Language>F#</Language>
    <Language>VB</Language>
    <Language>C++/CLI</Language>
    
    <Import>System.Reflection</Import>
    
    <!--
      "versionLabel" extracts numerical-notate version string [1.2.3.4] or [v1.2.3.4] from git repository tags traverse start HEAD.
      If not found, use [0.0.1].
    -->
    <Rule name="AssemblyVersion">{versionLabel}</Rule>
    
    <!--
      "safeVersion" is extracts committed date (from commmiter) from git repository HEAD.
      "safeVersion" specialized from "committer.When".
      (The format is safe-numerical-notate version string [2016.2.14.12345]. (Last number is 2sec prec.))
    -->
    <Rule name="AssemblyFileVersion">{safeVersion}</Rule>
    
    <!--
      "commitId" is extracts commit id from git repository HEAD.
      "commitId" alias to "commit.Sha".
    -->
    <Rule name="AssemblyInformationalVersion">{versionLabel}-{commitId}</Rule>
    
    <!--
      "key" attribute can only use with "AssemblyMetadataAttribute".
      "committer.When" or you can use another choice "author.When".
      "branch" can use property "FriendlyName" and "CanonicalName". (Derived from libgit2sharp)
      "author" and "committer" can use property "Name", "Email", and "When". (Derived from libgit2sharp)
      "buildIdentifier" is passing from MSBuild property named "RelaxVersionerBuildIdentifier" or "BuildIdentifier". We can use in CI building.
      "generated" is generated date by RelaxVersioner.
      You can apply format directives same as string.Format().
    -->
    <Rule name="AssemblyMetadata" key="Date">{committer.When:R}</Rule>
    <Rule name="AssemblyMetadata" key="Branch">{branch.FriendlyName}</Rule>
    <Rule name="AssemblyMetadata" key="Tags">{tags}</Rule>
    <Rule name="AssemblyMetadata" key="Author">{author}</Rule>
    <Rule name="AssemblyMetadata" key="Committer">{committer}</Rule>
    <Rule name="AssemblyMetadata" key="Message">{commit.MessageShort}</Rule>
    <Rule name="AssemblyMetadata" key="Build">{buildIdentifier}</Rule>
    <Rule name="AssemblyMetadata" key="Generated">{generated:R}</Rule>
    <Rule name="AssemblyMetadata" key="TargetFramework">{tfm}</Rule>
    
    <!-- These definitions are not included by defaults.
    <Rule name="AssemblyMetadata" key="TargetFrameworkIdentity">{tfid}</Rule>
    <Rule name="AssemblyMetadata" key="TargetFrameworkVersion">{tfv}</Rule>
    <Rule name="AssemblyMetadata" key="TargetFrameworkProfile">{tfp}</Rule>
    -->

    <!--
      The "Platform" identity is a MSBuild property name.
      You can use "Platform" and another identities come from PropertyGroup definitions
      and process environments such as "RootNamespace", "Prefer32Bit", "NETCoreSdkVersion", "PATH" and etc.
      Each results are strictly string type, so format directives will be ignored.
    -->
    <Rule name="AssemblyMetadata" key="Platform">{Platform}</Rule>
    <Rule name="AssemblyMetadata" key="BuildOn">{OS}</Rule>
    <Rule name="AssemblyMetadata" key="SdkVersion">{NETCoreSdkVersion}</Rule>

    <!-- These definitions are not included by defaults.
    <Rule name="AssemblyMetadata" key="Language">{Language}</Rule>
    <Rule name="AssemblyMetadata" key="HostName">{COMPUTERNAME}</Rule>
    -->
  </WriterRules>
</RelaxVersioner>
```

----

## TIPS

### SourceLink integration

[Sourcelink](https://github.com/dotnet/sourcelink) is a cool stuff debugger integration package for showing source code on-the-fly downloading from Git source code repository. 

RelaxVersioner already supported Sourcelink integration. You can integrate using both RelaxVersioner and Sourcelink on simple step:

```xml
<!-- Example common properties for Sourcelink integration -->
<PropertyGroup>
  <!-- Will embed project untracked source files -->
  <EmbedUntrackedSources>true</EmbedUntrackedSources>

  <!-- Symbol embedding is recommended -->
  <DebugType>embedded</DebugType>

  <!-- Or you have to include symbol files (*.pdb) into same package -->
  <!-- <DebugType>portable</DebugType> -->
  <!-- <AllowedOutputExtensionsInPackageBuildOutputFolder>.pdb</AllowedOutputExtensionsInPackageBuildOutputFolder> -->

  <!-- Required: Will include Git repository information into assembly -->
  <PublishRepositoryUrl>true</PublishRepositoryUrl>
  <RepositoryType>git</RepositoryType>
  <RepositoryUrl>https://github.com/kekyo/Epoxy.git</RepositoryUrl>
</PropertyGroup>

<!-- Will build deterministic binary on GitHub CI Release building -->
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <Deterministic>true</Deterministic>
  <ContinuousIntegrationBuild>true</ContinuousIntegrationBuild>
</PropertyGroup>

<ItemGroup>
  <!-- RelaxVersioner -->
  <PackageReference Include="RelaxVersioner" Version="2.5.5" PrivateAssets="All" />

  <!-- Root directory location of the solution file, if it exists. -->
  <!-- Refer: https://github.com/dotnet/roslyn/issues/37379 -->
  <SourceRoot Include="$(MSBuildThisFileDirectory)/"/>
</ItemGroup>

<!-- Required: Add Sourcelink package reference on only Release build -->
<ItemGroup Condition="'$(Configuration)' == 'Release'">
  <PackageReference Include="Microsoft.SourceLink.GitHub" Version="1.1.1" PrivateAssets="All" />
</ItemGroup>
```

For more further informations, [see Sourcelink documentation.](https://github.com/dotnet/sourcelink/blob/main/docs/README.md)

### CI process trouble shooting

If your CI process causes error with this description like:

```
RelaxVersioner[1.0.13.0]: NotFoundException=object not found -
   no match for id (a2b834535c00e7b1a604fccc28cfebe78ea0ec31),
   Unknown exception occurred, ...
```

It means your clone repository contains only 1 depth commit or some.
(And couldn't find these commit id.)
You must clone all commits into CI workspace.

For example, GitHub Actions `checkout@v2` task will clone only 1 depth for defaulted.
Because it makes better fast cloning.

RelaxVersioner (and other automated versioning tool) requires all commits for calculating version depth.
Apply `fetch-depth: 0` predication into your build.yml script.
[You can understand with this real script.](https://github.com/kekyo/CenterCLR.RelaxVersioner/blob/master/.github/workflows/build.yml#L11)

### Use nuspec file to generate NuGet package

When you are using a nuspec file to generate a NuGet package, there are additional symbols available besides the default placeholders, we can do automated building NuGet package with nuspec file. Please refer to the example below:

```xml
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
  <metadata>
    <!-- Embedding package version -->
    <version>$PackageVersion$</version>

    <!-- Embedding branch name and commit ID -->
    <repository type="git" url="https://example.com/your/repo.git" branch="$RepositoryBranch$" commit="$RepositoryCommit$" />

    <!-- Embedding commit message -->
    <releaseNotes>$PackageReleaseNotes$</releaseNotes>
  </metadata>
</package>
```

* Add the prepared nuspec file to the project and specify it with the `NuSpecFile` property inside of `PropertyGroup`. The rest of the procedure is the same as the general NuGet packaging procedure.

----

## Another topics

* RelaxVersioner supported on Visual Studio 2012/2013 only installed .NET Framework 4.6 or upper. Because it requires uses compatibility for net461 MSBuild.Framework assembly.

## TODO:

* Support exclude rule set.
* Support native C++ project.
* Support templated output.
* Support fallback rule set.

## License

* Copyright (c) 2015-2021 Kouji Matsui (@kozy_kekyo, @kekyo2)
* Under Apache v2

----

## History

* 2.5.5:
  * Fixed implicitly requirements for installation .NET Core 2.0 environment.
* 2.5.2:
  * In .NET 6 environment, fixed an issue where the default set of additional attributes was not being applied.
* 2.5.1:
  * Fixed causing undefined `BundledNETCoreAppTargetFrameworkVersion` at some environment.
* 2.5.0:
  * Upgraded VS2022 (Project doesn't change)
  * Fixed detection for building runtime version on NET5/6.
* 2.4.0:
  * Supported .NET 6.0 SDK.
* 2.3.2:
  * Fixed causing LibGit2SharpException built on VisualStudio for Mac.
* 2.3.0:
  * Fixed executing on only installed sdk3.1 or 5.0.
* 2.2.0:
  * Supported sourcelink annotations (AssemblyInformationalVersion format and NuGet packaging.)
* 2.1.0:
  * Added supporting a lot of configurable properties derived from MSBuild runtime time PropertyGroups. With MSBuild alone, it was quite a hassle to refer to these values, but you can just specify the property name in the rules file and embed the values ​​in the same way. (See custom rule set file section.)
* 2.0.9:
  * Fixed conflicting ValueTuple assembly at net461 binary.
* 2.0.8:
  * Fixed depth calculation for untagged repository.
* 2.0.5:
  * Fixed duplicates AssemblyMetadataAttribute class on C# and net40-client platform.
* 2.0.0:
  * Reached 2.0 🎉
  * Support .NET 5 and Linux environments.
  * Added static literals. You can refer all symbols inside of "ThisAssembly" class/module, doesn't require using any reflection API.
  * Added support keys "TargetFramework" (tfm), "TargetFrameworkIdentity" (tfid), "TargetFrameworkVersion" (tfv) and "TargetFrameworkProfile" (tfp). Only includes tfm by defaults. See also rule set file section.
  * Reduced package size.
  * Breaking change: Changed package naming from "CenterCLR.RelaxVersioner" to "RelaxVersioner". Old packages will make unlisting.
* 1.0.10:
  * Fixed failure aggregating referenced package versions (related changing nuget impls).
* 1.0.5:
  * Fixed always version 0.0.1 when doesn't apply first tag.
* 1.0.0:
  * Reached 1.0 🎉
  * Totally omitted MSBuild Task assemblies (because it makes many troubles :( RelaxVersioner is driven by independent commandline worker!
* 0.10.24:
  * Omitted net40/net45 platform because there caused conflicting version for MSBuild.Framework assemblies.
* 0.10.19:
  * Fixed using uninitialized repository.
* 0.10.17:
  * Improved fork analysis.
* 0.10.11:
  * Experimental supported for net40/net45 MSBuild platforms. (In VS2012-2013, but not tested)
  * Omitted referencing MSBuild utility assembly.
* 0.10.6:
  * Fixed using uninitialized repository.
* 0.10.3:
  * Added auto increment feature.
  * Removed applying from branch name.
  * Bit changed rule format (breaking change)
* 0.9.69:
  * Fixed contains runtime directory when building final result.
* 0.9.67:
  * Fixed failing extract branch name.
* 0.9.66:
  * Fixed can't extract version informations from git repository at linux environment.
  * Improved logging architectures.
  * Improved CI validating on .NET Core 2 environment.
* 0.9.62:
  * Validated dotnet cli on .NET Core 2 environment (Both Windows and Linux)
* 0.9.25:
  * Made self hosted versioning :)
  * Splitted devel and master branches.
  * Switched CI to Azure Pipelines and write configurations (WIP)
  * Begin validation for linux environment (WIP)
* 0.9.14:
  * Added developmentDependency attribute.
* 0.9.13:
  * Fixed failure loading native library by dotnet cli.
  * Added applying nuget package version feature. (dotnet pack)
* 0.9.1:
  * Upgraded handlers on the new MSBuild scripts (Formally .NET Core 2/.NET Standard projects)
  * Upgraded LibGit2Sharp 0.26.1/2.0.289
  * Sorry dropped WiX supporting.
* 0.8.30:
  * Added Import feature. (Thanks @biobox)
* 0.8.20:
  * Fixed adopt the first found rule set (Thanks @zizi4n5)
  * Fixed package attribute, set to the developmentDependency (Thanks @zizi4n5)
  * Fixed trimming the git label version start with 'v' prefix (Thanks @zizi4n5)
  * Improved NuGet package generation, switched to the MSBuild script (Thanks @biobox)
* 0.8.11:
  * Embed metadatas use for always self defined AssemblyVersionMetadataAttribute instead mscorlib::AssemblyMetadataAttribute.
  * Default versioning uses "0.0.1.0" for non-committed git repository.
  * Updated LibGit2Sharp to 0.22.0/1.0.129.
* 0.8.3:
  * Add Wix support. (WIP)
* 0.8.2:
  * Add mono support. (WIP, Please PR for complete :)
* 0.8.1:
  * Change ruleset element name "Rules" -> "WriterRules" (Breaking change)
  * Change "gitLabel" fallback value, "safeVersion" -> "0.0.0.0"
  * Output version information diagnostics on build time
  * Tabs fixed
* 0.7.18:
  * Fixed PCL target project cause AssemblyMetadataAttribute not found.
* 0.7.17:
  * Fixed not exist TargetPath folders on CI environments.
* 0.7.16:
  * Update libgit2sharp and NamingFormatter
* 0.7.14:
  * Add support SemVer's optional prefix/postfix.
  * Change git traverse strategy, tags retreive only HEAD commit.
* 0.7.13:
  * Fixing build fail included dirty string attributes. Example: Dirty\String"Test"
* 0.7.12:
  * Fix crashed when using non-committed repository.
* 0.7.11:
  * Add {branches} and {tags} format.
* 0.7.10:
  * Use NamingFormatter.
  * Use System.Version.Parse.
* 0.7.7:
  * Fixed lack of newest tag.
* 0.7.6:
  * Support lower .NET Framework (2.0-3.5)
* 0.7.5:
  * Traverse git repository between projectDirectory and the root.
  * Ignore not found/cannot open git repository.
* 0.7.1: Support custom rule set file.
* 0.5.30: Checked on 4 languages.
* 0.5.0: Initial public commit. (Broken, still under construction)
