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

        private readonly TaskLoggingHelper logger;

        private AssemblyLoader(TaskLoggingHelper logger) =>
            this.logger = logger;

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var path = Path.Combine(AssemblyLoadHelper.BasePath, assemblyName.Name + ".dll");
            if (File.Exists(path) && base.LoadFromAssemblyPath(path) is Assembly assembly)
            {
                logger.LogMessage(logImportance, "RelaxVersioner: Assembly loaded: Name={0}, Path={1}",
                    assemblyName,
                    new Uri(assembly.CodeBase, UriKind.RelativeOrAbsolute).LocalPath);
                return assembly;
            }

            if (Default.LoadFromAssemblyName(assemblyName) is Assembly assembly2)
            {
                logger.LogMessage(logImportance, "RelaxVersioner: Assembly loaded from default context: Name={0}, Path={1}",
                    assemblyName,
                    new Uri(assembly2.CodeBase, UriKind.RelativeOrAbsolute).LocalPath);
                return assembly2;
            }

            logger.LogWarning("RelaxVersioner: Cannot load assembly: Name={0}, Path={1}",
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
                    logger.LogMessage(logImportance, "RelaxVersioner: Native library loaded: Name={0}, Path={1}",
                        unmanagedDllName,
                        path);
                    return handle;
                }
                else
                {
                    logger.LogWarning("RelaxVersioner: Cannot load native library: Name={0}, Path={1}",
                        unmanagedDllName,
                        path);
                }
            }

            return IntPtr.Zero;
        }

        public static object Run<T>(TaskLoggingHelper logger, string methodName, params object[] args)
        {
            var loader = new AssemblyLoader(logger);

            var assembly = loader.LoadFromAssemblyPath(AssemblyLoadHelper.GetAssemblyPathDerivedFromBasePath(typeof(T).Assembly));
            var type = assembly.GetType(typeof(T).FullName);
            var method = type.GetMethod(methodName);
            return method.Invoke(null, args);
        }
    }
#endif
}
