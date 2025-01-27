#!/bin/bash

# RelaxVersioner - Git tag/branch based, full-automatic version generator.
# Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
#
# Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0

echo ""
echo "==========================================================="
echo "Build RelaxVersioner"
echo ""

# https://github.com/dotnet/msbuild/issues/1709
export MSBUILDDISABLENODEREUSE=1

dotnet build -p:Configuration=Release -p:Platform="Any CPU" -p:RestoreNoCache=True RelaxVersioner.sln
dotnet pack -p:Configuration=Release -p:Platform="Any CPU" -o artifacts RelaxVersioner.sln
