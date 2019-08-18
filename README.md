# RelaxVersioner
![RelaxVersioner](https://raw.githubusercontent.com/kekyo/CenterCLR.RelaxVersioner/master/Images/CenterCLR.RelaxVersioner.128.png)

[![Japanese language](https://raw.githubusercontent.com/kekyo/CenterCLR.RelaxVersioner/master/Images/Japanese.256.png)](https://github.com/kekyo/CenterCLR.RelaxVersioner/blob/master/README.ja.md)

# Status
* NuGet Package: [![NuGet RelaxVersioner](https://img.shields.io/nuget/v/CenterCLR.RelaxVersioner.svg?style=flat)](https://www.nuget.org/packages/CenterCLR.RelaxVersioner)
* Continuous integration: [![AppVeyor RelaxVersioner](https://img.shields.io/appveyor/ci/kekyo/centerclr-relaxversioner.svg?style=flat)](https://ci.appveyor.com/project/kekyo/centerclr-relaxversioner)
* Gitter: [![Gitter RelaxVersioner](https://img.shields.io/gitter/room/kekyo/CenterCLR.RelaxVersioner.svg?style=flat)](https://gitter.im/kekyo/CenterCLR.RelaxVersioner)

## What is this?
* RelaxVersioner is Very easy-usage, Git-based, auto-generate version informations in .NET Core/.NET Framework source code. (Assembly attribute based)
* If you use RelaxVersioner, version handling ONLY use for Git tags/branches/commit messages. Of course you don't need more tooling usage, and easy managing continuous-integration environments.
* Target language/projects: C#, F#, VB.NET and C++/CLI.
* Auto collect version information from local Git repository tags/branch name.
* Independent AssemblyInfo.cs file, RelaxVersioner is output into temporary file. (No direct manipulate AssemblyInfo file).
* Place source code location which isn't obstructive for Git. (ex: obj/Debug)
* You can customize output attribute/values with custom rule set file.

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

## How to use
* Search NuGet repository: "CenterCLR.RelaxVersioner", and install. https://www.nuget.org/packages/CenterCLR.RelaxVersioner/
* Before build, comment out "AssemblyVersion" and "AssemblyFileVersion" attribute in AssemblyInfo.cs default definitions (will cause build error by duplicate definition). If you use custom rule set, continue use this definitions.
* After installed, build project. Auto-apply version informations into assembly attributes. Some attributes are looking for directory Windows Explorer property page.
* You can use custom rule set file naming "RelaxVersioner.rules" into project folder "$(ProjectDir)" or solution folder "$(SolutionDir)". Current no documentation custom rule set file, see also below.

## Example for development cycles
1. You create new C#,F# or another project.
2. Install "RelaxVersioner" from NuGet.
3. Comment outs default "AssemblyVersion" and "AssemblyFileVersion" declarations in AssemblyInfo.cs.
4. Let's build now! Output binary applied version informations.
  * Default declaration of AssemblyVersion="0.0.1.0", AssemblyFileVersion="(Build date on 2sec prec.)" (ex:"2016.05.12.11523"）
  * Applied AssemblyVersionMetadata from local Git repository (Author, Branch, Tags）. But this example git repository not created , so there declarations containing "Unknown".
5. Create Git local repository (command: git init). And commit with message.
6. Build retry. Output binary applied Author, Branch, Tags now.
7. You are tagging current commit. For example "0.5.4". Build retry, contains AssemblyVersion is "0.5.4" now.
8. All rights codes, tags. Push to remote repository, all done.
9. Development cycles: next codes change and ready to release, you are tagging new version and then build, output binary auto tagged in AssemblyVersion and store informations.

## Sample custom rule set file:

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
			"gitLabel" is extract numerical-notate version string [1.2.3.4] from git repository tags/branches traverse start HEAD.
			If not found, fallback to "safeVersion".
		-->
		<Rule name="System.Reflection.AssemblyVersionAttribute">{gitLabel}</Rule>
		<!--
			"safeVersion" is extract committed date (with commmiter) from git repository HEAD.
			"safeVersion" specialized from "committer.When".
			(The format is safe-numerical-notate version string [2016.2.14.12345]. (Last number is 2sec prec.))
		-->
		<Rule name="System.Reflection.AssemblyFileVersionAttribute">{safeVersion}</Rule>
		<!--
			"commitId" is extract commit id from git repository HEAD.
			"commitId" alias to "commit.Sha".
		-->
		<Rule name="System.Reflection.AssemblyInformationalVersionAttribute">{commitId}</Rule>
		<!--
			"key" is only used "AssemblyVersionMetadataAttribute".
			"committer.When" or you can use another choice "author.When".
			"author" and "committer" can use property "Name", "Email", and "When". (Derived from libgit2sharp)
		-->
		<Rule name="System.Reflection.AssemblyVersionMetadataAttribute" key="Build">{committer.When:R}</Rule>
		<Rule name="System.Reflection.AssemblyVersionMetadataAttribute" key="Branch">{branch.Name}</Rule>
		<Rule name="System.Reflection.AssemblyVersionMetadataAttribute" key="Tags">{tags}</Rule>
		<Rule name="System.Reflection.AssemblyVersionMetadataAttribute" key="Author">{author}</Rule>
		<Rule name="System.Reflection.AssemblyVersionMetadataAttribute" key="Committer">{committer}</Rule>
		<Rule name="System.Reflection.AssemblyVersionMetadataAttribute" key="Message">{commit.MessageShort}</Rule>
	</WriterRules>
</RelaxVersioner>
```

## TODO:
* Support exclude rule set.
* Support native C++ project.
* Support NuGet output project. (nuproj?, nubuild? and/or other projects?)
* Support templated output.
* Support fallback rule set.
* Support Mono environments (and CI environments on *nix, Please PR for complete :)
  * For 0.9.1, I supported .NET Standard platform. So I think may already run on these environments. (But not verified)
* Switch CI to Azure Pipelines. 

## License
* Copyright (c) 2015-2019 Kouji Matsui (@kozy_kekyo, kekyo2)
* Under Apache v2

## History
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
