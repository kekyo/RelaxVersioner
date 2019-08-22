#!/bin/sh

# CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
# Copyright (c) 2016-2018 Kouji Matsui (@kozy_kekyo, @kekyo2)
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

dotnet clean -c Release -p:Platform=AnyCPU CenterCLR.RelaxVersioner.Tasks/CenterCLR.RelaxVersioner.Tasks.csproj
dotnet build -c Release -p:Platform=AnyCPU CenterCLR.RelaxVersioner.Tasks/CenterCLR.RelaxVersioner.Tasks.csproj

dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -o artifacts CenterCLR.RelaxVersioner.Tasks/CenterCLR.RelaxVersioner.Tasks.csproj
