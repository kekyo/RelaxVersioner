﻿/////////////////////////////////////////////////////////////////////////////////////////////////
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

namespace CenterCLR.RelaxVersioner.Loader
{
#if NETSTANDARD2_0
    
    using System.Reflection;

    internal sealed class AssemblyLoadContext : System.Runtime.Loader.AssemblyLoadContext
    {
        private static readonly string nativeDllName =
            (string)typeof(LibGit2Sharp.Repository).
                Assembly.
                GetType("LibGit2Sharp.Core.NativeDllName").
                GetField("Name", BindingFlags.Public | BindingFlags.Static).
                GetValue(null);

        private AssemblyLoadContext()
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var path = Path.Combine(AssemblyLoadHelper.BasePath, assemblyName.Name + ".dll");
            return File.Exists(path) ?
                this.LoadFromAssemblyPath(path) :
                Default.LoadFromAssemblyName(assemblyName);
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

        private static readonly AssemblyLoadContext instance = new AssemblyLoadContext();
    
        public static object Invoke<T>(string methodName, params object[] args)
        {
            var assembly = instance.LoadFromAssemblyPath(AssemblyLoadHelper.AssemblyPath);
            var type = assembly.GetType(typeof(T).FullName);
            var method = type.GetMethod(methodName);
            return method.Invoke(null, args);
        }
    }

#else

    using System.Linq;
    using Microsoft.DotNet.PlatformAbstractions;

    internal static class AssemblyLoadContext
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        private static extern bool AddDllDirectory(string path);

        private static bool initialized = false;
        private static readonly object initializeLocker = new object();

        private static void PrependBasePaths(string targetEnvironmentName, params string[] basePaths)
        {
            var pathEnvironment = (Environment.GetEnvironmentVariable(targetEnvironmentName) ?? string.Empty).Trim();
            var newPath = string.Join(Path.PathSeparator.ToString(), basePaths.Concat(new[] { pathEnvironment }));
            Environment.SetEnvironmentVariable(targetEnvironmentName, newPath);
        }

        private static void SetupEnvironmentsIfRequired()
        {
            if (!initialized)
            {
                lock (initializeLocker)
                {
                    if (!initialized)
                    {
                        // HACK: I know it's bad practice, but I don't take very complex implementation for using AppDomain.
                        switch (RuntimeEnvironment.OperatingSystemPlatform)
                        {
                            case Platform.Windows:
                                PrependBasePaths("PATH", AssemblyLoadHelper.BasePath, AssemblyLoadHelper.BaseNativePath);
                                if (int.TryParse(RuntimeEnvironment.OperatingSystemVersion.Split('.')[0], out var v) && (v >= 8))
                                {
                                    if (!AddDllDirectory(AssemblyLoadHelper.BasePath))
                                    {
                                        System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
                                    }
                                    if (!AddDllDirectory(AssemblyLoadHelper.BaseNativePath))
                                    {
                                        System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
                                    }
                                }
                                break;
                            default:
                                // NOTE: In macos, ElCapitan disabled dylib lookuping feature, so will cause loading failure.
                                PrependBasePaths("PATH", AssemblyLoadHelper.BasePath);
                                PrependBasePaths("LD_LIBRARY_PATH", AssemblyLoadHelper.BaseNativePath);
                                break;
                        }

                        initialized = true;
                    }
                }
            }
        }

        public static object Invoke<T>(string methodName, params object[] args)
        {
            SetupEnvironmentsIfRequired();

            var method = typeof(T).GetMethod(methodName);
            return method.Invoke(null, args);
        }
    }
#endif
}