@echo off

rem RelaxVersioner - Git tag/branch based, full-automatic version information inserter.
rem Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
rem
rem Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0

echo.
echo "==========================================================="
echo "Build RelaxVersioner"
echo.

rem https://github.com/dotnet/msbuild/issues/1709
set MSBUILDDISABLENODEREUSE=1

dotnet build -p:Configuration=Release -p:Platform="Any CPU" --no-cache RelaxVersioner.sln
dotnet pack -p:Configuration=Release -p:Platform="Any CPU" -o artifacts RelaxVersioner.sln
