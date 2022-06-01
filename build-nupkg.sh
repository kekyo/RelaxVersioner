#!/bin/sh

# RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
# Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
#
# Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0

echo ""
echo "==========================================================="
echo "Build CenterCLR.RelaxVersioner"
echo ""

dotnet restore
dotnet build -c Release -p:Platform="Any CPU" CenterCLR.RelaxVersioner.sln
# dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -p:NoWarn=NU5104 -o artifacts CenterCLR.RelaxVersioner.Core/CenterCLR.RelaxVersioner.Core.csproj
dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -o artifacts CenterCLR.RelaxVersioner/CenterCLR.RelaxVersioner.csproj
