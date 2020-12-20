# RelaxVersioner
![RelaxVersioner](Images/CenterCLR.RelaxVersioner.128.png)

[![Japanese language](Images/Japanese.256.png)](https://github.com/kekyo/CenterCLR.RelaxVersioner/blob/master/README.ja.md)

# Status

[![Project Status: Active – The project has reached a stable, usable state and is being actively developed.](https://www.repostatus.org/badges/latest/active.svg)](https://www.repostatus.org/#active)

| | master | devel |
|:---|:---|:---|
| Packages | [![NuGet RelaxVersioner (master)](https://img.shields.io/nuget/v/RelaxVersioner.svg?style=flat)](https://www.nuget.org/packages/RelaxVersioner) | [![MyGet RelaxVersioner (devel)](https://img.shields.io/myget/kekyo/v/RelaxVersioner.svg?style=flat&label=myget)](https://www.myget.org/feed/kekyo/package/nuget/RelaxVersioner)
| Continuous integration | [![RelaxVersioner CI build (master)](https://github.com/kekyo/CenterCLR.RelaxVersioner/workflows/.NET/badge.svg?branch=master)](https://github.com/kekyo/CenterCLR.RelaxVersioner/actions) | [![RelaxVersioner CI build (devel)](https://github.com/kekyo/CenterCLR.RelaxVersioner/workflows/.NET/badge.svg?branch=master)](https://github.com/kekyo/CenterCLR.RelaxVersioner/actions)

## What is this?
* RelaxVersioner is a easy, full-automatic, git based version inserter for .NET 5/.NET Core/.NET Framework.
* If you use RelaxVersioner, version handling ONLY use with Git tags/branches/commit messages. Of course you don't need more tooling usage, and easy integrates continuous-integration environments.
* Target language/environments:
  * C#, F#, VB.NET, C++/CLI and NuGet packaging (dotnet cli packer).
  * Visual Studio 2019/2017/2015, dotnet SDK cli, MSBuild on netcoreapp2.1/net461 environment (NOT your project platform) and related IDEs.
  * Linux(x64) and Windows(x86/x64).  (The project validates only them, but maybe can run at same as [libgit2sharp](https://github.com/libgit2/libgit2sharp) required environment)
* Auto collect version information from local Git repository tags/branch name.
* Independent AssemblyInfo.cs file, generated code will output into a temporary file. (Not manipulate directly AssemblyInfo.cs file).
* Place source code location which isn't obstructive for Git. (ex: obj/Debug)
* You can customize output attribute/values with custom rule set file.

### Result for assembly property at the explorer

![Result for assembly property at the explorer](Images/Explorer.png)

### Result for assembly wide attributes at ILSpy

![Assembly wide attributes at ILSpy](Images/ILSpy.png)

## Sample output codes

### For C#:

``` csharp
using System.Reflection;
[assembly: AssemblyVersion("0.5.30.0")]
[assembly: AssemblyFileVersion("2016.1.15.41306")]
[assembly: AssemblyInformationalVersion("a05ab9fc87b22234596f4ddd43136e9e526ebb90")]
[assembly: AssemblyMetadata("Build","Fri, 15 Jan 2016 13:56:53 GMT")]
[assembly: AssemblyMetadata("Branch","master")]
[assembly: AssemblyMetadata("Tags","0.5.30")]
[assembly: AssemblyMetadata("Author","Kouji Matsui <k@kekyo.net>")]
[assembly: AssemblyMetadata("Committer","Kouji Matsui <k@kekyo.net>")]
[assembly: AssemblyMetadata("Message","Fixed tab")]
[assembly: AssemblyMetadata("Message","Fixed tab")]
[assembly: AssemblyMetadata("TargetFramework", "net461")]

namespace YourApp
{
  internal static class ThisAssembly
  {
    public const string AssemblyVersion = "0.5.30.0";
    public const string AssemblyFileVersion = "2016.1.15.41306";
    public const string AssemblyInformationalVersion = "a05ab9fc87b22234596f4ddd43136e9e526ebb90";
// TODO:
  }
}
```

### For F#:

``` fsharp
namespace global
  open System.Reflection
  [<assembly: AssemblyVersion("0.5.30.0")>]
  [<assembly: AssemblyFileVersion("2016.1.15.41306")>]
  [<assembly: AssemblyInformationalVersion("a05ab9fc87b22234596f4ddd43136e9e526ebb90")>]
  [<assembly: AssemblyMetadata("Build","Fri, 15 Jan 2016 13:56:53 GMT")>]
  [<assembly: AssemblyMetadata("Branch","master")>]
  [<assembly: AssemblyMetadata("Tags","0.5.30")>]
  [<assembly: AssemblyMetadata("Author","Kouji Matsui <k@kekyo.net>")>]
  [<assembly: AssemblyMetadata("Committer","Kouji Matsui <k@kekyo.net>")>]
  [<assembly: AssemblyMetadata("Message","Fixed tab")>]
  do()
// TODO:
```

### For VB.NET:

``` visualbasic
Imports System.Reflection
<Assembly: AssemblyVersion("0.5.30.0")>
<Assembly: AssemblyFileVersion("2016.1.15.41306")>
<Assembly: AssemblyInformationalVersion("a05ab9fc87b22234596f4ddd43136e9e526ebb90")>
<Assembly: AssemblyMetadata("Build","Fri, 15 Jan 2016 13:56:53 GMT")>
<Assembly: AssemblyMetadata("Branch","master")>
<Assembly: AssemblyMetadata("Tags","0.5.30")>
<Assembly: AssemblyMetadata("Author","Kouji Matsui <k@kekyo.net>")>
<Assembly: AssemblyMetadata("Committer","Kouji Matsui <k@kekyo.net>")>
<Assembly: AssemblyMetadata("Message","Fixed tab")>
' TODO:
```

### For C++/CLI:

``` cpp
using namespace System::Reflection;
[assembly: AssemblyVersion("0.5.30.0")];
[assembly: AssemblyFileVersion("2016.1.15.41306")];
[assembly: AssemblyInformationalVersion("a05ab9fc87b22234596f4ddd43136e9e526ebb90")];
[assembly: AssemblyMetadata("Build","Fri, 15 Jan 2016 13:56:53 GMT")];
[assembly: AssemblyMetadata("Branch","master")];
[assembly: AssemblyMetadata("Tags","0.5.30")];
[assembly: AssemblyMetadata("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyMetadata("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyMetadata("Message","Fixed tab")];
// TODO:
```

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
      "versionLabel" is extract numerical-notate version string [1.2.3.4] or [v1.2.3.4] from git repository tags traverse start HEAD.
      If not found, use [0.0.1].
    -->
    <Rule name="AssemblyVersion">{versionLabel}</Rule>
    
    <!--
      "safeVersion" is extract committed date (from commmiter) from git repository HEAD.
      "safeVersion" specialized from "committer.When".
      (The format is safe-numerical-notate version string [2016.2.14.12345]. (Last number is 2sec prec.))
    -->
    <Rule name="AssemblyFileVersion">{safeVersion}</Rule>
    
    <!--
      "commitId" is extract commit id from git repository HEAD.
      "commitId" alias to "commit.Sha".
    -->
    <Rule name="AssemblyInformationalVersion">{commitId}</Rule>
    
    <!--
      "key" attribute can only use with "AssemblyMetadataAttribute".
      "committer.When" or you can use another choice "author.When".
      "branch" can use property "FriendlyName" and "CanonicalName". (Derived from libgit2sharp)
      "author" and "committer" can use property "Name", "Email", and "When". (Derived from libgit2sharp)
      "buildIdentifier" is passing from MSBuild property named "RelaxVersionerBuildIdentifier" or "BuildIdentifier". We can use in CI building.
      "generated" is generated date by RelaxVersioner.
    -->
    <Rule name="AssemblyMetadata" key="Date">{committer.When:R}</Rule>
    <Rule name="AssemblyMetadata" key="Branch">{branch.FriendlyName}</Rule>
    <Rule name="AssemblyMetadata" key="Tags">{tags}</Rule>
    <Rule name="AssemblyMetadata" key="Author">{author}</Rule>
    <Rule name="AssemblyMetadata" key="Committer">{committer}</Rule>
    <Rule name="AssemblyMetadata" key="Message">{commit.MessageShort}</Rule>
    <Rule name="AssemblyMetadata" key="Build">{buildIdentifier}</Rule>
    <Rule name="AssemblyMetadata" key="Generated">{generated:R}</Rule>
  </WriterRules>
</RelaxVersioner>
```

## TIPS

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
[You can understand with this real script.](https://github.com/kekyo/CenterCLR.RelaxVersioner/blob/master/.github/workflows/build.yml#L13)

## Another topics
* RelaxVersioner supported on Visual Studio 2012/2013 only installed .NET Framework 4.6 or upper. Because it requires uses compatibility for net461 MSBuild.Framework assembly.

## TODO:
* Support exclude rule set.
* Support native C++ project.
* Support templated output.
* Support fallback rule set.
* Support Mono on linux environments.

## License
* Copyright (c) 2015-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
* Under Apache v2

## History
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
