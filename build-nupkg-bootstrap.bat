@echo off

rem RelaxVersioner - Git tag/branch based, full-automatic version generator.
rem Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
rem
rem Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0

rem This script is used by the RelaxVersioner to get over when .NET releases a new major version.
rem For example .NET 8.0 release, the RelaxVersioner that uses itself will fail to build
rem because it must support the `net8.0` TFM, which was not included in earlier versions.
rem This script will generate a RelaxVersioner with a temporary version embedded
rem and a private package so that it can build itself with the new .NET version.

rem `PackageVersion` should be set appropriately.

echo.
echo "==========================================================="
echo "Build RelaxVersioner (Bootstrap)"
echo.

rem https://github.com/dotnet/msbuild/issues/1709
set MSBUILDDISABLENODEREUSE=1

dotnet build -p:Configuration=Release -p:Platform="Any CPU" -p:BOOTSTRAP=True -p:PackageVersion=3.2.10 -p:RestoreNoCache=True RelaxVersioner.sln
dotnet pack -p:Configuration=Release -p:Platform="Any CPU" -p:BOOTSTRAP=True -p:PackageVersion=3.2.10 -o artifacts RelaxVersioner.sln
