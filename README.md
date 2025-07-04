# RelaxVersioner

![RelaxVersioner](Images/RelaxVersioner.128.png)

[![Japanese language](Images/Japanese.256.png)](https://github.com/kekyo/RelaxVersioner/blob/master/README.ja.md)

# Status

[![Project Status: Active – The project has reached a stable, usable state and is being actively developed.](https://www.repostatus.org/badges/latest/active.svg)](https://www.repostatus.org/#active) 

|Package|Link|
|:----|:----|
|RelaxVersioner (MSBuild)|[![NuGet RelaxVersioner (MSBuild)](https://img.shields.io/nuget/v/RelaxVersioner.svg?style=flat)](https://www.nuget.org/packages/RelaxVersioner)|
|rv-cli (CLI)|[![NuGet RelaxVersioner (CLI)](https://img.shields.io/nuget/v/rv-cli.svg?style=flat)](https://www.nuget.org/packages/rv-cli)|

## What is this?

Have you ever wanted to embed the current version of .NET, its documentation, etc.?
Furthermore, have you ever wanted to automate such an operation with CI, but found the additional work to be too opaque and numerous?

In C#, such information is conventionally described in the `AssemblyInfo.cs` file:

```csharp
using System.Reflection;

// Embed version number in assembly attribute.
[assembly: AssemblyVersion("1.0.21")]
```

However, this is as far as the standard goes.
It was the developer's responsibility to properly update the embedded version number.

RelaxVersioner makes version embedding extremely simple by applying versions via Git tagging.
Simply put, you can tag a Git tag with `1.0.21` and it will automatically generate and embed an assembly attribute like the one shown above!

The assembly with the embedded version number can be partially viewed in the Properties page of Explorer:

![Assembly property at the explorer](Images/Explorer.png)

If you look in ILSpy, you will also see all the information:

![Assembly wide attributes at ILSpy](Images/ILSpy.png)

* Simply install RelaxVersioner's NuGet package into your .NET project and it will embed the version number from your Git tags completely automatically at build time.
  * Don't worry about untagged commits. It can go back in time, find the tag, and apply the automatically incremented version.
* This is not just a tag. It also embeds additional information such as commit ID, branch name, author name, date and etc. The format can be freely changed.
* You can also customize the information to be embedded. For example, by embedding the value of variables used in MSBuild, you can also embed detailed information at build time.
* Version embedding is fully integrated into MSBuild. This means that you can simply install the NuGet package and embed versions directly in the build process of IDEs such as Visual Studio, Rider, Visual Studio Code or in the build process of CI.
  * RelaxVersioner does not contain any environment-dependent code, so it works in almost any .NET environment.
* A CLI interface also exists. You can embed version numbers in text documents and apply the same version notation to different project systems than .NET (e.g. `Makefile` and etc.).
  * CLI has a dedicated NPM (Node.js package manager) mode, which allows you to easily centralize NPM project and version operations.

## Sample output code

The following is a C# example, but F#, VB.net, and C++/CLI are also supported. The output attributes are the same.

In addition to the assembly attributes, a `ThisAssembly` symbol is also defined.
This has the advantage that you can easily retrieve various version information without having to use reflection to search for the attributes:

``` csharp
using System.Reflection;

[assembly: AssemblyVersion("1.0.21")]
[assembly: AssemblyFileVersion("2020.12.20.33529")]
[assembly: AssemblyInformationalVersion("1.0.21-561387e2f6dc90046f56ef4c3ac501aad0d5ec0a")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyMetadata("AssemblyName","YourApp")]
[assembly: AssemblyMetadata("TargetFrameworkMoniker","net6.0")]
[assembly: AssemblyMetadata("Date","Sunday, April 23, 2023 9:42:21 PM 0900")]
[assembly: AssemblyMetadata("Branch","master")]
[assembly: AssemblyMetadata("Tags","")]
[assembly: AssemblyMetadata("Author","Kouji Matsui <k@kekyo.net>")]
[assembly: AssemblyMetadata("Committer","Kouji Matsui <k@kekyo.net>")]
[assembly: AssemblyMetadata("Subject","Merge branch 'devel'")]
[assembly: AssemblyMetadata("Body","")]
[assembly: AssemblyMetadata("Build","")]
[assembly: AssemblyMetadata("Generated","Sunday, April 23, 2023 9:42:21 PM 0900")]
[assembly: AssemblyMetadata("Platform","AnyCPU")]
[assembly: AssemblyMetadata("BuildOn","Unix")]
[assembly: AssemblyMetadata("SdkVersion","7.0.100")]
[assembly: AssemblyMetadata("ApplicationVersion","33529")]
[assembly: AssemblyMetadata("ApplicationDisplayVersion","1.0.21")]

namespace YourApp
{
  internal static class ThisAssembly
  {
    public const string AssemblyVersion = "1.0.21";
    public const string AssemblyFileVersion = "2020.12.20.33529";
    public const string AssemblyInformationalVersion = "1.0.21-561387e2f6dc90046f56ef4c3ac501aad0d5ec0a";
    public const string AssemblyConfiguration = "Release";
    public static class AssemblyMetadata
    {
      public const string AssemblyName = "YourApp";
      public const string TargetFrameworkMoniker = "net6.0";
      public const string Date = "Sunday, April 23, 2023 9:42:21 PM 0900";
      public const string Branch = "master";
      public const string Tags = "";
      public const string Author = "Kouji Matsui <k@kekyo.net>";
      public const string Committer = "Kouji Matsui <k@kekyo.net>";
      public const string Subject = "Merge branch 'devel'";
      public const string Body = "";
      public const string Build = "";
      public const string Generated = "Sunday, April 23, 2023 9:42:21 PM 0900";
      public const string Platform = "AnyCPU";
      public const string BuildOn = "Unix";
      public const string SdkVersion = "7.0.100";
      public const string ApplicationVersion = "33529";
      public const string ApplicationDisplayVersion = "1.0.21";
    }
  }
}
```

----

## Getting started

### Start guide

* [An easy way to practice version embedding on .NET (dev.to)](https://dev.to/kozy_kekyo/an-easy-way-to-practice-version-embedding-on-net-45h8)
* [Refer start guides.](./STARTGUIDE.md)

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

## Command line interface (CLI)

RelaxVersioner supports the dotnet CLI tool.
The `rv` command can be installed with:

```bash
$ dotnet tool install -g rv-cli
```

You can easily obtain the version string by using the `rv` command as follows:

```bash
$ rv .
1.2.3.4
```

You can use `-f` to get different output. For example:

```bash
# Standard version format
$ rv -f "{versionLabel}" .
1.2.3.4

# Commit ID
$ rv -f "{commitId}" .
01234567899abc ...

# Complex formats are also possible
$ rv -f "{commitId}:{commitDate:yyyyMMdd}/{branches}" .
0123456789 ...:20240123/develop,main
```

* Please refer to below section such as `versionLabel`, `commitId` and etc. that are specified as placeholders.
  * Note: `tfm`, `tfid`, `tfv`, `tfp`, `namespace` and `buildIdentifier` cannot be recognized unless they are specified as command line options.

You can use the `-o` option to output directly to a file.
In this case, no newlines are included at the end of the line:

```bash
# Output to the file
$ rv -o version.txt .
$ cat version.txt
1.2.3.4
```

If you use the `-r` or `-i` options, you will be in “replace mode”, which allows you to directly replace arbitrary text.
Using this mode, you can perform bulk text replacements in a more direct way:

```bash
# Replace text on standard input
$ echo "@@@{commitId}@@@" | rv -r .
@@@0123456789abc ... @@@

# Replace specified file text
$ rv -i input.txt -o output.txt .
```

The standard bracket for the replacement keyword is in the form of braces '{ ... }', but different bracket characters can be used.
For example, when applied to a document, braces can cause false positives, so a custom bracket is used to replace them:

```bash
# Replace with custom brackets
# Separate left and right bracket definitions with commas
$ echo "ABC#{commitId}#XYZ" | rv -r --bracket "#{,}#" .
ABC0123456789abc ... XYZ
```

With this CLI, you can use a combination of RelaxVersioner for different targets than .NET.
For example, in a CI/CD environment like GitHub Actions, you can apply versions to NPM package generation or embed versions in your text documentation.


----

## Using with Node.js projects

The CLI interface has special options for NPM (Node.js package manager).
Change to the directory where the `package.json` file is located and run the CLI as follows:

```bash
$ rv --npm .
RelaxVersioner [3.8.0]: Generated versions code: Language=NPM, Version=12.34.56
```

The value of the `version` key in `package.json` will then be replaced with the correct version value.

* This function is similar to the standard NPM `npm version` command.
* The version search algorithm is based on RelaxVersioner, so it is useful if you want to share the same version with .NET projects.

Furthermore, if you use the `--npmns` option, you can also change the package versions that the package depends on at the same time.
For example, if you are managing multiple packages and there are dependencies between the packages:

```json
{
  "name": "webapi-interaction",
  "version": "0.0.1",             // <-- The version of this package (before updating)
  "dependencies": {
    "@foobar/helpers": "^1.4.0",  // <-- Version of the managed package that is a dependency
    "@foobar/common": "^1.7.2",   // <-- Version of the managed package that is a dependency
    "dayjs": "^1.2.3"
  }
}
```

Run the CLI specifying the namespace of the package name (`@foobar`) that is a dependency:

```bash
$ rv --npmns @foobar .
```

This will unify all the version numbers:

```json
{
  "name": "webapi-interaction",
  "version": "13.24.5",             // <-- Automatically updated
  "dependencies": {
    "@foobar/helpers": "^13.24.5",  // <-- automatically updated (dependency)
    "@foobar/common": "^13.24.5",   // <-- automatically updated (dependency)
    "dayjs": "^1.2.3"               // <-- not updated
  }
}
```

* Dependency checks are performed on the `dependencies`, `peerDependencies` and `devDependencies` keys.
* Please use a consistent package namespace to identify dependent packages.
  * In practice, this is a simple prefix match, so any name that can be identified is fine.
  * If you are targeting multiple namespaces, specify them separated by commas.
* The version of a dependent package is automatically prefixed with `^`.

This function is intended to be used with [NPM workspaces](https://docs.npmjs.com/cli/v7/using-npm/workspaces) and inside the CI process.
In that case, please follow the NPM workspaces handling, such as using `*` as the version number of the referenced package.
After updating the version of each package, run `npm install` in the workspace root directory to update the correct version numbers in `package-lock.json`.


----

## Hints and Tips

### Reflect the working directory status

When using RelaxVersioner in a .NET project or with the option `--cwd` in the CLI, the version number increment will take into account whether or not changes have been made to the working directory.

For example, if the version being determined is `1.2.3` and there are (uncommitted) changes made to the working directory, the version applied will be `1.2.4`.

.NET projects, this behavior does not recursively affect Git changes, since the version number is never directly embedded in the source code. Therefore, it always considers the working directory status by default.

It is disabled by default in the CLI interface. In particular, if you specify `--npm` and apply it to NPM's `package.json`, you will have a situation where the version number is reapplied by the RelaxVersioner, which causes the version change to occur again (causing a difference in Git).

The `--cwd` option is still valid in this case, but care must be taken because the version number will not be reflected in `package-lock.json` unless you run `npm install` immediately afterwards.

### How to use version numbers after building process

RelaxVersioner saves the files in the following location after build:

```
<your project dir>/obj/<configuration>/<tfm>/
```

To be precise:

* `$(IntermediateOutputPath)` at build time.
* `$(NuspecOutputPath)` at NuGet package generation.

For example, `FooBarProject/obj/Debug/net6.0/` directory hierarchy. Here are the files to save:

* `RelaxVersioner_Metadata.cs` : Source code including version attributes and `ThisAssembly` class definition, which is the core feature of RelaxVersioner.
* `RelaxVersioner_Properties.xml` : A dump of all MSBuild properties in XML format, just before RelaxVersioner calculates the version.
* `RelaxVersioner_Result.xml` : An XML dump of the main version information after RelaxVersioner has calculated the version.
* `RelaxVersioner_Version.txt` : A text file containing only the version number after RelaxVersioner has calculated the version.
* `RelaxVersioner_ShortVersion.txt` : A text file containing only the short version number after RelaxVersioner has calculated the version.
* `RelaxVersioner_SafeVersion.txt` : A text file containing only the safe (safe-numerical-notate) version number after RelaxVersioner has calculated the version.
* `RelaxVersioner_Branch.txt` : a text file containing the name of the checkout branch from the time RelaxVersioner has calculated the version.
* `RelaxVersioner_Tags.txt` : A text file containing the tags corresponding to the commits when RelaxVersioner has calculated the version.

If you want to reference this information from your program, you can get it from the version attributes or `ThisAssembly`. Other, XML or text files can be referenced by CI/CD (Continuous Integration and Continuous Deployment) to apply version information to the build process. For example, `RelaxVersioner_ShortVersion.txt` contains a string like `2.5.4`, so you may be able to save your build artifacts with a version number when you upload them to the server.

If you want to reference this information from within the MSBuild target, you can use the properties as follows without having to access the text file:

```xml
  <Target Name="AB" AfterTargets="Compile">
    <Message Importance="High" Text="PropertiesPath: $(RelaxVersionerPropertiesPath)" />
    <Message Importance="High" Text="ResultPath: $(RelaxVersionerResultPath)" />
    <Message Importance="High" Text="ResolvedVersion: $(RelaxVersionerResolvedVersion)" />
    <Message Importance="High" Text="ResolvedShortVersion: $(RelaxVersionerResolvedShortVersion)" />
    <Message Importance="High" Text="ResolvedSafeVersion: $(RelaxVersionerResolvedSafeVersion)" />
    <Message Importance="High" Text="ResolvedIntDateVersion: $(RelaxVersionerResolvedIntDateVersion)" />
    <Message Importance="High" Text="ResolvedEpochIntDateVersion: $(RelaxVersionerResolvedEpochIntDateVersion)" />
    <Message Importance="High" Text="ResolvedCommitId: $(RelaxVersionerResolvedCommitId)" />
    <Message Importance="High" Text="ResolvedBranch: $(RelaxVersionerResolvedBranch)" />
    <Message Importance="High" Text="ResolvedTags: $(RelaxVersionerResolvedTags)" />
  </Target>
```

`RelaxVersioner_Properties.xml` contains a lot of very useful information, and you may be able to pull information from this XML file to meet your specific needs without having to write custom MSBuild scripts.

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
  <PackageReference Include="RelaxVersioner" Version="2.12.0" PrivateAssets="All" />

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
RelaxVersioner [2.12.0]: NotFoundException=object not found -
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
[You can understand with this real script.](https://github.com/kekyo/RelaxVersioner/blob/master/.github/workflows/build.yml#L11)

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
        "safeVersion" extracts committed date (from commmiter) from git repository HEAD.
        (The format is safe-numerical-notate version string [2016.2.14.12345]. (Last number is 2sec prec.))
    -->
    <Rule name="AssemblyFileVersion">{safeVersion}</Rule>

    <!--
        "commitId" extracts commit id from git repository HEAD.
        "commitId" alias to "commit.Hash".
    -->
    <Rule name="AssemblyInformationalVersion">{versionLabel}-{commitId}</Rule>

    <Rule name="AssemblyConfiguration">{Configuration}</Rule>

    <!--
        "key" attribute can only use with "AssemblyMetadataAttribute".
        "branch" can use field "Name". (Derived from GitReader)
        "author" and "committer" can use field "Name", "MailAddress", and "Date". (Derived from GitReader)
        "buildIdentifier" is passing from MSBuild property named "RelaxVersionerBuildIdentifier" or "BuildIdentifier".
        We can use in CI building.
        "generated" is generated date by RelaxVersioner.
        You can apply format directives same as string.Format().
    -->
    <Rule name="AssemblyMetadata" key="CommitId">{commitId}</Rule>
    <Rule name="AssemblyMetadata" key="Date">{commitDate:F} {commitDate.Offset:hhmm}</Rule>
    <Rule name="AssemblyMetadata" key="Branches">{branches}</Rule>
    <Rule name="AssemblyMetadata" key="Tags">{tags}</Rule>
    <Rule name="AssemblyMetadata" key="Author">{author}</Rule>
    <Rule name="AssemblyMetadata" key="Committer">{committer}</Rule>
    <Rule name="AssemblyMetadata" key="Subject">{commit.Subject}</Rule>
    <Rule name="AssemblyMetadata" key="Body">{commit.Body}</Rule>
    <Rule name="AssemblyMetadata" key="Build">{buildIdentifier}</Rule>
    <Rule name="AssemblyMetadata" key="Generated">{generated:F}</Rule>
    <Rule name="AssemblyMetadata" key="TargetFramework">{tfm}</Rule>

    <!--
        Both "ApplicationVersion" and "ApplicationDisplayVersion" are used for .NET MAUI versioning.
        "epochIntDateVersion" is a value calculated in the same way as `safeVersion`.
    -->
    <Rule name="AssemblyMetadata" key="ApplicationDisplayVersion">{shortVersion}</Rule>
    <Rule name="AssemblyMetadata" key="ApplicationVersion">{epochIntDateVersion}</Rule>

    <!--
        The "Platform" identity is a MSBuild property name.
        You can use "Platform" and another identities come from PropertyGroup definitions
        and process environments such as "RootNamespace", "Prefer32Bit", "NETCoreSdkVersion", "PATH" and etc.
        Each results are strictly string type, so format directives will be ignored.
    -->
    <Rule name="AssemblyMetadata" key="AssemblyName">{AssemblyName}</Rule>
    <Rule name="AssemblyMetadata" key="RootNamespace">{RootNamespace}</Rule>
    <Rule name="AssemblyMetadata" key="PlatformTarget">{PlatformTarget}</Rule>
    <Rule name="AssemblyMetadata" key="Platform">{Platform}</Rule>
    <Rule name="AssemblyMetadata" key="RuntimeIdentifier">{RuntimeIdentifier}</Rule>
    <Rule name="AssemblyMetadata" key="BuildOn">{OS}</Rule>
    <Rule name="AssemblyMetadata" key="SdkVersion">{NETCoreSdkVersion}</Rule>
  </WriterRules>
</RelaxVersioner>
```

----

## License

* Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
* Under Apache v2

----

## History

* 3.18.0:
  * Updated GitReader to 1.16.0 and uses primitive-interface instead of structured-interface.
* 3.17.0:
  * Included untracked files to cwd mode.
  * Updated GitReader to 1.14.0.
* 3.16.0:
  * Made working directory checks run in parallel.
  * Made working directory checks optionally specifiable.
* 3.15.0:
  * Updated GitReader to 1.13.0 to improve working directory search performance (#20).
  * Improved detection for invalid version tags format.
* 3.14.0:
  * Version number is incremented when there are files being modified in the working directory.
* 3.13.0:
  * Fixed generate correct source code even when symbol names and literal strings contain control characters.
* 3.12.0:
  * Changed to strictly parse the version number range as a 16-bit value.
  * Changed to allow more than one version notation (i.e., `1.2` or `1.2.3`).
  * Updated GitReader to 1.10.0 (Git worktree support).
* 3.11.0:
  * Updated GitReader to 1.9.0.
* 3.10.0:
  * Fixed a bug that prevented building on .NET 9.0 SDK or upper, depending on the environment.
* 3.9.0:
  * Changed UTF8 text file output to no BOM.
* 3.8.0:
  * Added NPM mode, which automatically inserts the version of NPM `package.json`.
  * Fixed version number print garbage in the build log when building NuGet packages.
* 3.7.0:
  * CLI now supports custom bracket specification.
* 3.6.0:
  * Plain text output format on CLI can now be specified
  * Plain text on CLI can now be output to standard output.
  * Added replace mode on CLI.
  * Simplified the CLI version options to work with fewer specifications.
* 3.5.0:
  * Added for specify the format of plain text output.
  * Plain text can now be output to standard output.
  * Replace mode has been added.
  * The CLI version options have been simplified to improve functionality with fewer specifications.
* 3.4.0:
  * Fixed a problem with getting correct information when a project is placed inside a Git submodule.
  * Updated GitReader to 1.8.0.
* 3.3.0:
  * Added support for .NET 8.0 SDK.
  * Updated GitReader to 1.7.0.
* 3.2.60:
  * Fixed the `ThisAssembly` symbol could not be referenced at build time.
* 3.2.50:
  * Improved to use the highest version number when multiple version tags are included in the same commit.
* 3.2.40:
  * .NET 8.0 RC2 is now supported. Although it can probably be used for the .NET 8.0 release without any modification, but will release a rebuilt version after .NET 8.0 is released.
* 3.2.20:
  * .NET 8.0 RC1 is now supported. Although it can probably be used for the .NET 8.0 release without any modification, but will release a rebuilt version after .NET 8.0 is released.
* 3.2.0:
  * Updated GitReader to 1.4.0.
* 3.1.0:
  * Changed default rule `AssemblyMetadata.TargetFramework` to `AssemblyMetadata.TargetFrameworkMoniker`.
  * Fixed `AssemblyMetadata.ApplicationVersion` exceeding 65535.
  * Add `AssemblyMetadata.RootNamespace`.
* 3.0.0:
  * A multi-branch tracking analyzer has been implemented.
    Previously, it searched past commits in order from the primary branch and determined the version number based on the first version number found.
    Now automatically find the largest version number in all branches involved.
    It will automatically determine the correct version without having to manually search for the last largest version number.
    (No special action is required for this change.)
  * GitReader has been raised to 1.1.0.
* 2.16.0:
  * Fixed causing NRE on uninitialized git repository.
* 2.15.0:
  * Changed reading of Git repositories using [GitReader](https://github.com/kekyo/GitReader) instead of libgit2sharp.
    The native library no longer depends on it, which eases the limitation of the operating environment.
  * The format of some rule files has been changed. See the diff.
* 2.14.0:
  * Supported .NET 7 SDK.
  * Application version information (`ApplicationDisplayVersion`, `ApplicationVersion`) used in .NET MAUI is now supported.
    The latter is seconds from epoch by default.
* 2.13.1:
  * Fixed `Attribute` in `AssemblyConfiguration` static definition name.
  * Fixed incorrect base position of SourceLink (in RelaxVersioner itself);
    Your projects generated using RelaxVersioner are not affected.
* 2.13.0:
  * Supported pseudo tfm on older project format.
  * Added more usable assembly configuration metadata.
* 2.12.1:
  * Expanded version result text files.
    Results for `RelaxVersioner_SafeVersion.txt`, `RelaxVersioner_Branch.txt`, and `RelaxVersioner_Tags.txt` are available.
  * The version result text file does not contain line feed codes.
  * `ShortVersion` is now fallback assigned to the MSBuild property `Version` immediately after a build.
    Subsequent MSBuild scripts that use the `Version` property can safely use the recognized version number.
* 2.11.0:
  * Fixed causing error on NuGet packaging when contains no source code input files.
* 2.10.0:
  * Fixed doesn't update package version on project referenced targets.
* 2.9.0:
  * Fixed causing XML file reading errors in projects that did not generate any outputs.
* 2.8.0:
  * Added output of a text file containing only the version, which can be used externally.
  * Adjusted build timing to fix a problem that sometimes prevented Intellisense from recognizing the `ThisAssembly` class.
  * Fixed a problem that sometimes prevented builds from executing when building NuGet packages.
* 2.7.0:
  * Defined file dependency rules at build time to avoid unnecessary builds.
* 2.6.0:
  * Improved clean build stability.
  * The development environments of `netcoreapp2.0`, `2.1` and `netcoreapp3.0` are no longer supported.
    If you are using these SDKs, please update to the latest SDK, respectively.
  * Improved showing banner on current target framework moniker.
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
  * Breaking change: Changed package naming from "RelaxVersioner" to "RelaxVersioner". Old packages will make unlisting.
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
