#!/bin/sh

# CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
# Copyright (c) 2016-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
# 
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# 
# http://www.apache.org/licenses/LICENSE-2.0
# 
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

echo ""
echo "==========================================================="
echo "Build CenterCLR.RelaxVersioner"
echo ""

dotnet restore
dotnet build -c Release -p:Platform="Any CPU" CenterCLR.RelaxVersioner.sln
# dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -p:NoWarn=NU5104 -o artifacts CenterCLR.RelaxVersioner.Core/CenterCLR.RelaxVersioner.Core.csproj
dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -o artifacts CenterCLR.RelaxVersioner/CenterCLR.RelaxVersioner.csproj
