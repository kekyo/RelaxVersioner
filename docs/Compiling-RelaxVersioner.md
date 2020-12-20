## Compiling Relax Versioner
### On Windows and Linux

To compile on Windows 10 or Linux (Ubuntu 20.04) using .NET SDK, make sure you have gotten the following:

* .NET 2.1, 3.1 and 5 SDK
* You don't need C++/CLI and VB.NET building environment when build RelaxVersioner itself.

The TestProjects.sln is required full installation of Visual Studio 2019.
But it's only for building the test projects and are thus optional.

You can refer the steps how to build RelaxVersioner:

* [`build-nupkg`](/build-nupkg.sh) script. It contains building NuGet package from clean source code.
* [`build.yml`](/.github/workflows/build.yml`) script. It contains building on the GitHub Actions.

### Exporting the Relax Versioner Package

To export this as a nuget package on Windows, simply run the `build-nupkg.bat` file.
The `build-nupkg.sh` is usable script on the Linux environemt.
