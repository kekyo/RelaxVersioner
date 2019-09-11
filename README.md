# RelaxVersioner
![RelaxVersioner](Images/CenterCLR.RelaxVersioner.128.png)

[![Japanese language](Images/Japanese.256.png)](https://github.com/kekyo/CenterCLR.RelaxVersioner/blob/master/README.ja.md)

# Status
| | master | devel |
|:---|:---|:---|
| Packages | [![NuGet RelaxVersioner (master)](https://img.shields.io/nuget/v/CenterCLR.RelaxVersioner.svg?style=flat)](https://www.nuget.org/packages/CenterCLR.RelaxVersioner) | [![MyGet RelaxVersioner (devel)](https://img.shields.io/myget/kekyo/v/CenterCLR.RelaxVersioner.svg?style=flat&label=myget)](https://www.myget.org/feed/kekyo/package/nuget/CenterCLR.RelaxVersioner)
| Continuous integration | [![Azure pipelines (master)](https://kekyo.visualstudio.com/CenterCLR.RelaxVersioner/_apis/build/status/CenterCLR.RelaxVersioner-master)](https://kekyo.visualstudio.com/CenterCLR.RelaxVersioner/_build?definitionId=10) | [![Azure pipelines (devel)](https://kekyo.visualstudio.com/CenterCLR.RelaxVersioner/_apis/build/status/CenterCLR.RelaxVersioner-devel)](https://kekyo.visualstudio.com/CenterCLR.RelaxVersioner/_build?definitionId=9)

## What is this?
* RelaxVersioner is Very easy-usage, Git-based, auto-generate version informations in .NET Core/.NET Framework source code. (Assembly attribute based)
* If you use RelaxVersioner, version handling ONLY use for Git tags/branches/commit messages. Of course you don't need more tooling usage, and easy managing continuous-integration environments.
* Target language/environments:
  * C#, F#, VB.NET, C++/CLI and NuGet packaging (dotnet cli pack).
  * Visual Studio 2019/2017/2015, dotnet SDK cli, MSBuild on netstandard2.0/net46 (NOT your project platform) and related IDEs.
  * Linux(x64) and Windows(x86/x64).  (The project validates only them, but maybe runs same as [libgit2sharp](https://github.com/libgit2/libgit2sharp) required environment)
* Auto collect version information from local Git repository tags/branch name.
* Independent AssemblyInfo.cs file, RelaxVersioner is output into temporary file. (No direct manipulate AssemblyInfo file).
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
[assembly: AssemblyVersionAttribute("0.5.30.0")]
[assembly: AssemblyFileVersionAttribute("2016.1.15.41306")]
[assembly: AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")]
[assembly: AssemblyVersionMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")]
[assembly: AssemblyVersionMetadataAttribute("Branch","master")]
[assembly: AssemblyVersionMetadataAttribute("Tags","0.5.30")]
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")]
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")]
[assembly: AssemblyVersionMetadataAttribute("Message","Fixed tab")]
```

### For F#:

``` fsharp
namespace global
  open System.Reflection
  [<assembly: AssemblyVersionAttribute("0.5.30.0")>]
  [<assembly: AssemblyFileVersionAttribute("2016.1.15.41306")>]
  [<assembly: AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")>]
  [<assembly: AssemblyVersionMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")>]
  [<assembly: AssemblyVersionMetadataAttribute("Branch","master")>]
  [<assembly: AssemblyVersionMetadataAttribute("Tags","0.5.30")>]
  [<assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")>]
  [<assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")>]
  [<assembly: AssemblyVersionMetadataAttribute("Message","Fixed tab")>]
  do()
```

### For VB.NET:

``` visualbasic
Imports System.Reflection
<Assembly: AssemblyVersionAttribute("0.5.30.0")>
<Assembly: AssemblyFileVersionAttribute("2016.1.15.41306")>
<Assembly: AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")>
<Assembly: AssemblyVersionMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")>
<Assembly: AssemblyVersionMetadataAttribute("Branch","master")>
<Assembly: AssemblyVersionMetadataAttribute("Tags","0.5.30")>
<Assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")>
<Assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")>
<Assembly: AssemblyVersionMetadataAttribute("Message","Fixed tab")>
```

### For C++/CLI:

``` cpp
using namespace System::Reflection;
[assembly: AssemblyVersionAttribute("0.5.30.0")];
[assembly: AssemblyFileVersionAttribute("2016.1.15.41306")];
[assembly: AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")];
[assembly: AssemblyVersionMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","master")];
[assembly: AssemblyVersionMetadataAttribute("Tags","0.5.30")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Fixed tab")];
```

## Getting started

### Start guide
[Refer start guides.](./STARTGUIDE.md)

### How to use
* Search NuGet repository: ["RelaxVersioner"](https://www.nuget.org/packages/CenterCLR.RelaxVersioner/), and install.
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
7. You are tagging current commit. For example "0.5.4". Build retry, contains AssemblyVersion is "0.5.4" now.
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
    <Rule name="AssemblyVersionAttribute">{versionLabel}</Rule>
    
    <!--
      "safeVersion" is extract committed date (from commmiter) from git repository HEAD.
      "safeVersion" specialized from "committer.When".
      (The format is safe-numerical-notate version string [2016.2.14.12345]. (Last number is 2sec prec.))
    -->
    <Rule name="AssemblyFileVersionAttribute">{safeVersion}</Rule>
    
    <!--
      "commitId" is extract commit id from git repository HEAD.
      "commitId" alias to "commit.Sha".
    -->
    <Rule name="AssemblyInformationalVersionAttribute">{commitId}</Rule>
    
    <!--
      "key" attribute is only using for "AssemblyVersionMetadataAttribute".
      "committer.When" or you can use another choice "author.When".
      "branch" can use property "FriendlyName" and "CanonicalName". (Derived from libgit2sharp)
      "author" and "committer" can use property "Name", "Email", and "When". (Derived from libgit2sharp)
      "buildIdentifier" is passing from MSBuild property named "RelaxVersionerBuildIdentifier" or "BuildIdentifier". We can use in CI building.
      "generated" is generated date by RelaxVersioner.
    -->
    <Rule name="AssemblyVersionMetadataAttribute" key="Date">{committer.When:R}</Rule>
    <Rule name="AssemblyVersionMetadataAttribute" key="Branch">{branch.FriendlyName}</Rule>
    <Rule name="AssemblyVersionMetadataAttribute" key="Tags">{tags}</Rule>
    <Rule name="AssemblyVersionMetadataAttribute" key="Author">{author}</Rule>
    <Rule name="AssemblyVersionMetadataAttribute" key="Committer">{committer}</Rule>
    <Rule name="AssemblyVersionMetadataAttribute" key="Message">{commit.MessageShort}</Rule>
    <Rule name="AssemblyVersionMetadataAttribute" key="Build">{buildIdentifier}</Rule>
    <Rule name="AssemblyVersionMetadataAttribute" key="Generated">{generated:R}</Rule>
  </WriterRules>
</RelaxVersioner>
```

## TODO:
* Support exclude rule set.
* Support native C++ project.
* Support templated output.
* Support fallback rule set.
* Support Mono on linux environments.

## License
* Copyright (c) 2015-2019 Kouji Matsui (@kozy_kekyo, kekyo2)
* Under Apache v2

## History
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
