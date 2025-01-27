#!/bin/bash

# RelaxVersioner - Git tag/branch based, full-automatic version generator.
# Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
#
# Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0

# This script is used by the RelaxVersioner to get over when .NET releases a new major version.
# For example .NET 8.0 release, the RelaxVersioner that uses itself will fail to build
# because it must support the `net8.0` TFM, which was not included in earlier versions.
# This script will generate a RelaxVersioner with a temporary version embedded
# and a private package so that it can build itself with the new .NET version.

# `PackageVersion` should be set appropriately.

echo ""
echo "==========================================================="
echo "Build RelaxVersioner (Bootstrap)"
echo ""

# https://github.com/dotnet/msbuild/issues/1709
export MSBUILDDISABLENODEREUSE=1

dotnet build -p:Configuration=Release -p:Platform="Any CPU" -p:BOOTSTRAP=True -p:PackageVersion=3.2.10 -p:RestoreNoCache=True RelaxVersioner.sln
dotnet pack -p:Configuration=Release -p:Platform="Any CPU" -p:BOOTSTRAP=True -p:PackageVersion=3.2.10 -o artifacts RelaxVersioner.sln
