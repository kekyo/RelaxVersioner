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
using System.Linq;
using System.Reflection;

using Microsoft.Build.Utilities;
using Microsoft.DotNet.PlatformAbstractions;

namespace CenterCLR.RelaxVersioner
{
    internal static class AssemblyLoadHelper
    {
        public static readonly string EnvironmentIdentifier;
        public static readonly string NativeRuntimeIdentifier;
        public static readonly string BasePath;
        public static readonly string BaseNativePath;
        public static readonly string NativeExtension;

        static AssemblyLoadHelper()
        {
            BasePath = Path.GetDirectoryName(
                (new Uri(typeof(AssemblyLoadHelper).Assembly.CodeBase, UriKind.RelativeOrAbsolute)).LocalPath);

            var relaxVersionerVersion = typeof(AssemblyLoadHelper).Assembly.GetName().Version;
            var platformIdentifier = BasePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Last();
            var runtimeIdentifier = RuntimeEnvironment.GetRuntimeIdentifier();

            // "0.9.45/netstandard2.0/win10-x64"
            EnvironmentIdentifier = $"{relaxVersionerVersion}/{platformIdentifier}/{runtimeIdentifier}";

            var baseNativePath = Path.GetFullPath(Path.Combine(BasePath, "..", "..", "runtimes"));

            var id = runtimeIdentifier.Replace("-aot", string.Empty);
            var ids = id.Split('-');
            var shortPlatform0 = ids[0].Split('.')[0].Trim('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            var shortPlatform = string.Join("-", new[] { shortPlatform0 }.Concat(ids.Skip(1).Take(ids.Length - 2)));

            // "win-x64"
            NativeRuntimeIdentifier = $"{shortPlatform}-{RuntimeEnvironment.RuntimeArchitecture}";

            BaseNativePath = Path.Combine(baseNativePath, NativeRuntimeIdentifier, "native");

            switch (RuntimeEnvironment.OperatingSystemPlatform)
            {
                case Platform.Windows:
                    NativeExtension = ".dll";
                    break;
                case Platform.Darwin:
                    NativeExtension = ".dylib";
                    break;
                default:
                    NativeExtension = ".so";
                    // Will fallback not exist if platform is linux.
                    if (!Directory.Exists(BaseNativePath))
                    {
                        BaseNativePath = Path.Combine(baseNativePath, $"linux-{RuntimeEnvironment.RuntimeArchitecture}", "native");
                    }
                    break;
            }
        }

        public static string GetAssemblyPathDerivedFromBasePath(Assembly assembly) =>
            Path.Combine(BasePath, Path.GetFileName(new Uri(assembly.CodeBase, UriKind.RelativeOrAbsolute).LocalPath));

        private static bool initialized = false;
        private static readonly object initializeLocker = new object();

        private static void PrependBasePaths(string targetEnvironmentName, params string[] basePaths)
        {
            var pathEnvironment = (Environment.GetEnvironmentVariable(targetEnvironmentName) ?? string.Empty).Trim();
            var newPath = string.Join(Path.PathSeparator.ToString(), basePaths.Concat(new[] { pathEnvironment }));
            Environment.SetEnvironmentVariable(targetEnvironmentName, newPath);
        }

        private static void LoadNativeLibraries(
            TaskLoggingHelper logger, string basePath, string match, Func<string, IntPtr> loader)
        {
            var files = Directory.GetFiles(basePath, match, SearchOption.TopDirectoryOnly);
            if (files.Length >= 1)
            {
                foreach (var path in files)
                {
                    try
                    {
                        if (loader(path) == IntPtr.Zero)
                        {
                            System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(
                                System.Runtime.InteropServices.Marshal.GetHRForLastWin32Error());

                            logger.LogWarning("RelaxVersioner[{0}]: Cannot preload native library: Path={1}",
                                EnvironmentIdentifier,
                                path);
                        }
                        else
                        {
                            logger.LogMessage(Microsoft.Build.Framework.MessageImportance.High, "RelaxVersioner[{0}]: Native library preloaded: Path={1}",
                                EnvironmentIdentifier,
                                path);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning("RelaxVersioner[{0}]: Cannot preload native library: Path={1}, {2}",
                            EnvironmentIdentifier,
                            path,
                            ex.Message);
                    }
                }
            }
            else
            {
                logger.LogMessage(Microsoft.Build.Framework.MessageImportance.High, "RelaxVersioner[{0}]: Couldn't find native libraries: Path={1}{2}{3}",
                    EnvironmentIdentifier,
                    basePath,
                    Path.DirectorySeparatorChar,
                    match);
            }
        }

        public static void SetupNativeLibraries(TaskLoggingHelper logger)
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
                                PrependBasePaths("PATH", BasePath, BaseNativePath);
                                if (int.TryParse(RuntimeEnvironment.OperatingSystemVersion.Split('.')[0], out var v) && (v >= 8))
                                {
                                    NativeMethods.Win32_TryAddDllDirectory(BasePath);
                                    NativeMethods.Win32_TryAddDllDirectory(BaseNativePath);
                                }
                                LoadNativeLibraries(logger, BaseNativePath, "*" + NativeExtension, NativeMethods.Win32_LoadLibrary);
                                break;
                            case Platform.Darwin:
                                // NOTE: In macos, ElCapitan disabled dylib lookuping feature, so will cause loading failure.
                                PrependBasePaths("PATH", BasePath);
                                PrependBasePaths("DYLD_LIBRARY_PATH", BaseNativePath);
                                LoadNativeLibraries(logger, BaseNativePath, "*" + NativeExtension, path => NativeMethods.Unix_LoadLibrary(path, 2));
                                break;
                            default:
                                PrependBasePaths("PATH", BasePath);
                                PrependBasePaths("LD_LIBRARY_PATH", BaseNativePath);
                                LoadNativeLibraries(logger, BaseNativePath, "*" + NativeExtension, path => NativeMethods.Unix_LoadLibrary(path, 2));
                                break;
                        }

                        initialized = true;
                    }
                }
            }
        }
    }
}
