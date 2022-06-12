@echo off

rem RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
rem Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
rem
rem Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0

echo.
echo "==========================================================="
echo "Build RelaxVersioner"
echo.

msbuild -t:restore -p:Configuration=Release CenterCLR.RelaxVersioner.build.sln
msbuild -t:build -p:Configuration=Release CenterCLR.RelaxVersioner.build.sln
msbuild -t:pack -p:Configuration=Release -p:PackageOutputPath=..\artifacts CenterCLR.RelaxVersioner\CenterCLR.RelaxVersioner.csproj
