/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016-2019 Kouji Matsui (@kozy_kekyo, @kekyo2)
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

using System;
using System.IO;
using System.Reflection;

namespace CenterCLR.RelaxVersioner.Loader
{
#if NETSTANDARD2_0
    internal sealed class AssemblyLoadContext : System.Runtime.Loader.AssemblyLoadContext
    {
        private static readonly string nativeDllName =
            (string)typeof(LibGit2Sharp.Repository).
                Assembly.
                GetType("LibGit2Sharp.Core.NativeDllName").
                GetField("Name", BindingFlags.Public | BindingFlags.Static).
                GetValue(null);

        public static readonly AssemblyLoadContext Instance = new AssemblyLoadContext();

        private AssemblyLoadContext()
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var path = Path.Combine(AssemblyLoadHelper.BasePath, assemblyName.Name + ".dll");
            return File.Exists(path) ?
                this.LoadFromAssemblyPath(path) :
                System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            if (!Path.GetFileNameWithoutExtension(unmanagedDllName).EndsWith(nativeDllName))
            {
                return base.LoadUnmanagedDll(unmanagedDllName);
            }

            var path = Path.Combine(AssemblyLoadHelper.BaseNativePath, unmanagedDllName);
            if (!File.Exists(path))
            {
                path = Path.Combine(AssemblyLoadHelper.BaseNativePath, "lib" + unmanagedDllName);
            }

            var handle = base.LoadUnmanagedDllFromPath(path);
            return (handle != IntPtr.Zero) ?
                handle :
                base.LoadUnmanagedDll(unmanagedDllName);
        }
    }
#endif
}
