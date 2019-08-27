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

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CenterCLR.RelaxVersioner
{
#if NETSTANDARD2_0
    internal sealed class AssemblyLoader : System.Runtime.Loader.AssemblyLoadContext
    {
        private static readonly MessageImportance logImportance = MessageImportance.High;

        private static readonly object loaderLocker = new object();
        private static AssemblyLoader loader;

        private TaskLoggingHelper logger;

        private AssemblyLoader()
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Workaround failing invalid cast at the marshaller on ligit2sharp:
            //   https://github.com/dotnet/coreclr/issues/19654
            // But can't fix only this fragments, have to preload native library (in SetupNativeLibraries).
            if (StringComparer.InvariantCultureIgnoreCase.Equals(assemblyName.Name, "libgit2sharp"))
            {
                var a = typeof(LibGit2Sharp.Repository).Assembly;
                logger.LogMessage(logImportance, "RelaxVersioner[{0}]: Assembly libgit2sharp already loaded: Name={1}, Path={2}",
                    AssemblyLoadHelper.EnvironmentIdentifier,
                    assemblyName,
                    new Uri(a.CodeBase, UriKind.RelativeOrAbsolute).LocalPath);
                return a;
            }

            var path = Path.Combine(AssemblyLoadHelper.BasePath, assemblyName.Name + ".dll");
            if (File.Exists(path) && base.LoadFromAssemblyPath(path) is Assembly assembly)
            {
                logger.LogMessage(logImportance, "RelaxVersioner[{0}]: Assembly loaded: Name={1}, Path={2}",
                    AssemblyLoadHelper.EnvironmentIdentifier,
                    assemblyName,
                    new Uri(assembly.CodeBase, UriKind.RelativeOrAbsolute).LocalPath);
                return assembly;
            }

            if (Default.LoadFromAssemblyName(assemblyName) is Assembly assembly2)
            {
                logger.LogMessage(logImportance, "RelaxVersioner[{0}]: Assembly loaded from default context: Name={1}, Path={2}",
                    AssemblyLoadHelper.EnvironmentIdentifier,
                    assemblyName,
                    new Uri(assembly2.CodeBase, UriKind.RelativeOrAbsolute).LocalPath);
                return assembly2;
            }

            logger.LogWarning("RelaxVersioner[{0}]: Cannot load assembly: Name={1}, Path={2}",
                AssemblyLoadHelper.EnvironmentIdentifier,
                assemblyName,
                path);
            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            foreach (var prefixSuffix in new[] {
                new[] { string.Empty, AssemblyLoadHelper.NativeExtension },
                new[] { string.Empty, string.Empty },
                new[] { "lib", AssemblyLoadHelper.NativeExtension },
                new[] { "lib", string.Empty },
            })
            {
                var path = Path.Combine(AssemblyLoadHelper.BaseNativePath, prefixSuffix[0] + unmanagedDllName + prefixSuffix[1]);
                if (File.Exists(path) && base.LoadUnmanagedDllFromPath(path) is IntPtr handle)
                {
                    logger.LogMessage(logImportance, "RelaxVersioner[{0}]: Native library loaded: Name={1}, Path={2}",
                        AssemblyLoadHelper.EnvironmentIdentifier,
                        unmanagedDllName,
                        path);
                    return handle;
                }
            }

            logger.LogWarning("RelaxVersioner[{0}]: Cannot load native library: Name={1}, BasePath={2}",
                AssemblyLoadHelper.EnvironmentIdentifier,
                unmanagedDllName,
                AssemblyLoadHelper.BaseNativePath);

            return IntPtr.Zero;
        }

        public static object Run<T>(TaskLoggingHelper logger, string methodName, params object[] args)
        {
            AssemblyLoadHelper.SetupNativeLibraries(logger);

            if (loader == null)
            {
                lock (loaderLocker)
                {
                    if (loader == null)
                    {
                        loader = new AssemblyLoader();
                    }
                }
            }

            loader.logger = logger;

            var assembly = loader.LoadFromAssemblyPath(AssemblyLoadHelper.GetAssemblyPathDerivedFromBasePath(typeof(T).Assembly));
            var type = assembly.GetType(typeof(T).FullName);
            var method = type.GetMethod(methodName);
            return method.Invoke(null, args);
        }
    }
#endif
}
