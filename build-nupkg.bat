@echo off

rem CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
rem Copyright (c) 2016-2018 Kouji Matsui (@kozy_kekyo, @kekyo2)
rem 
rem Licensed under the Apache License, Version 2.0 (the "License");
rem you may not use this file except in compliance with the License.
rem You may obtain a copy of the License at
rem 
rem http://www.apache.org/licenses/LICENSE-2.0
rem 
rem Unless required by applicable law or agreed to in writing, software
rem distributed under the License is distributed on an "AS IS" BASIS,
rem WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
rem See the License for the specific language governing permissions and
rem limitations under the License.

dotnet clean -c Release -p:Platform=AnyCPU CenterCLR.RelaxVersioner.Tasks\CenterCLR.RelaxVersioner.Tasks.csproj
dotnet build -c Release -p:Platform=AnyCPU CenterCLR.RelaxVersioner.Tasks\CenterCLR.RelaxVersioner.Tasks.csproj

dotnet pack -p:PackageVersion=0.9.21 -p:Configuration=Release -p:Platform=AnyCPU -o artifacts CenterCLR.RelaxVersioner.Tasks\CenterCLR.RelaxVersioner.Tasks.csproj
