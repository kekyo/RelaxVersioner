## Compiling Relax Versioner
### On Windows

To compile on Windows using Visual Studio, make sure you have gotten the following through visual studio installer:

* .NET Framework (must support C# 6.0 or higher)
* Nuget package manager.
* MSBuild
* C# and Visual Basic language support
* F# language support
* Visual C++ 2017 Version
* C++/CLI

The last three are only for building the test projects and are thus optional. 

### On other systems (Mono)

Unfortunately, RelaxVersioner does not currently compile on Linux due to [this](https://github.com/libgit2/libgit2sharp/issues/1585) issue with **libgit2sharp**, a crucial dependency.
The compilation process shouldn't be that much different once it has been fixed.

### Exporting the Relax Versioner Package

To export this as a nuget package on Windows, simply run the `build-nupkg.bat` file.

For other systems, run the following command in the same directory as `CenterCLR.RelaxVersioner.csproj`:

```
xbuild /target:ExportNugetPackage
```
