/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016 Kouji Matsui (@kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/////////////////////////////////////////////////////////////////////////////////////////////////

using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("CenterCLR.RelaxVersioner")]
[assembly: AssemblyDescription(".NET source code level versioning toolset")]
[assembly: AssemblyCompany("Kouji Matsui")]
[assembly: AssemblyProduct("CenterCLR.RelaxVersioner")]
[assembly: AssemblyCopyright("Copyright (c) 2016 Kouji Matsui")]
[assembly: AssemblyTrademark("RelaxVersioner")]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#endif

[assembly: ComVisible(false)]
[assembly: Guid("0b01c9d5-a685-4444-981b-e8b4d6bdbbf1")]

[assembly: AssemblyVersion("0.7.6.0")]
[assembly: AssemblyFileVersion("0.7.6.0")]
