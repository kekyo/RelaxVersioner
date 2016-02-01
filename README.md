# RelaxVersioner
![RelaxVersioner](https://raw.githubusercontent.com/kekyo/CenterCLR.RelaxVersioner/master/Images/CenterCLR.RelaxVersioner.128.png)
* ![Japanese language](https://raw.githubusercontent.com/kekyo/CenterCLR.RelaxVersioner/master/Images/Japanese.256.png) https://github.com/kekyo/CenterCLR.RelaxVersioner/blob/master/README.ja.md

## What is this?
* RelaxVersioner is Very easy-usage, Git-based, auto-generate version informations in .NET source code. (Assembly attribute based)
* Target language/projects: C#, F#, VB.NET, and C++/CLI
* Auto collect version information from local Git repository tags/branch name.
* Independent AssemblyInfo.cs file, RelaxVersioner is output into temporary file. (No direct manipulate AssemblyInfo file).
* Place source code location which isn't obstructive for Git. (ex: obj/Debug)
* You can customize output attribute/values with custom rule set file.

## Sample output codes

### For C#:
``` csharp
[assembly: System.Reflection.AssemblyVersionAttribute("0.5.30")]
[assembly: System.Reflection.AssemblyFileVersionAttribute("2016.1.15.41306")]
[assembly: System.Reflection.AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Branch","master")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Message","Fixed tab")]
```

### For F#:
``` fsharp
namespace global
	[<assembly: System.Reflection.AssemblyVersionAttribute("0.5.30")>]
	[<assembly: System.Reflection.AssemblyFileVersionAttribute("2016.1.15.41306")>]
	[<assembly: System.Reflection.AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")>]
	[<assembly: System.Reflection.AssemblyMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")>]
	[<assembly: System.Reflection.AssemblyMetadataAttribute("Branch","master")>]
	[<assembly: System.Reflection.AssemblyMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")>]
	[<assembly: System.Reflection.AssemblyMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")>]
	[<assembly: System.Reflection.AssemblyMetadataAttribute("Message","Fixed tab")>]
	do()
```

### For VB.NET:
``` visualbasic
Imports System.Reflection
<Assembly: System.Reflection.AssemblyVersionAttribute("0.5.30")>
<Assembly: System.Reflection.AssemblyFileVersionAttribute("2016.1.15.41306")>
<Assembly: System.Reflection.AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Branch","master")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Message","Fixed tab")>
```

### For C++/CLI:
``` cpp
[assembly: System::Reflection::AssemblyVersionAttribute("0.5.30")];
[assembly: System::Reflection::AssemblyFileVersionAttribute("2016.1.15.41306")];
[assembly: System::Reflection::AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Branch","master")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Message","Fixed tab")];

```

## How to use
* Search NuGet repository: "CenterCLR.RelaxVersioner", and install. https://www.nuget.org/packages/CenterCLR.RelaxVersioner/
* Before build, comment out "AssemblyVersion" and "AssemblyFileVersion" attribute in AssemblyInfo.cs default definitions (will cause build error by duplicate definition). If you use custom rule set, continue use this definitions.
* After installed, build project. Auto-apply version informations into assembly attributes. Some attributes are looking for directory Windows Explorer property page.
* You can use custom rule set file naming "RelaxVersioner.rules" into project folder "$(ProjectDir)" or solution folder "$(SolutionDir)". Current no documentation custom rule set file, see also below.

## Sample custom rule set file:
``` xml
<?xml version="1.0" encoding="utf-8"?>
<RelaxVersioner version="1.0">
	<Rules>
		<!-- Target languages -->
		<Language>C#</Language>
		<Language>F#</Language>
		<Language>VB</Language>
		<Language>C++/CLI</Language>
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
			"key" is only used "AssemblyMetadataAttribute".
			If you use "AssemblyMetadataAttribute" and platform version == "v4.0",
			cannot use mscorlib implementation.
			Will auto define pseudo "AssemblyMetadataAttribute" class.
		-->
		<!--
			"committer.When" or you can use another choice "author.When".
			"author" and "committer" can use property "Name", "Email", and "When".
		-->
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Build">{committer.When:R}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Branch">{branch.Name}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Author">{author}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Committer">{committer}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Message">{commit.MessageShort}</Rule>
	</Rules>
</RelaxVersioner>
```

## TODO:
* Known problem : Not valid Git tag search algorithm.
* Support exclude rule set.
* Support native C++ project.

## License
* Copyright (c) 2015 Kouji Matsui (@kekyo2)
* Under Apache v2

## History
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
