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

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.DotNet.PlatformAbstractions;

namespace CenterCLR.RelaxVersioner
{
    internal static class AssemblyLoadHelper
    {
        public static string EnvironmentIdentifier { get; private set; }
        public static string NativeRuntimeIdentifier { get; private set; }
        public static string BasePath { get; private set; }
        public static string NativeExtension { get; private set; }
        public static string NativeLibraryPath { get; private set; }

        public static void Initialize(TaskLoggingHelper log)
        {
            if (BasePath != null)
            {
                return;
            }

            BasePath = Path.GetDirectoryName(
                (new Uri(typeof(AssemblyLoadHelper).Assembly.CodeBase, UriKind.RelativeOrAbsolute)).LocalPath);

            var relaxVersionerVersion = typeof(AssemblyLoadHelper).Assembly.GetName().Version;
            var platformIdentifier = BasePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Last();
            var runtimeIdentifier = RuntimeEnvironment.GetRuntimeIdentifier();

            // "0.9.45/netstandard2.0/win10-x64"
            EnvironmentIdentifier = $"{relaxVersionerVersion}/{platformIdentifier}/{runtimeIdentifier}";

            var baseNativeBasePath = Path.GetFullPath(Path.Combine(BasePath, "..", "..", "runtimes"));

            var id = runtimeIdentifier.Replace("-aot", string.Empty);
            var ids = id.Split('-');
            var shortPlatform0 = ids[0].Split('.')[0].Trim('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            var shortPlatform = string.Join("-", new[] { shortPlatform0 }.Concat(ids.Skip(1).Take(ids.Length - 2)));

            // "win-x64"
            NativeRuntimeIdentifier =
                (RuntimeEnvironment.OperatingSystemPlatform == Platform.Darwin) ?
                shortPlatform :
                $"{shortPlatform}-{RuntimeEnvironment.RuntimeArchitecture}";

            var baseNativePath0 = Path.Combine(baseNativeBasePath, NativeRuntimeIdentifier, "native");
            string[] baseNativePaths;
            Func<string, IntPtr> loader;

            switch (RuntimeEnvironment.OperatingSystemPlatform)
            {
                case Platform.Windows:
                    NativeExtension = ".dll";
                    baseNativePaths = new[] { baseNativePath0 };
                    loader = NativeMethods.Win32_LoadLibrary;
                    break;
                case Platform.Darwin:
                    NativeExtension = ".dylib";
                    baseNativePaths = new[] { baseNativePath0 };
                    loader = NativeMethods.Unix_LoadLibrary;
                    break;
                default:
                    NativeExtension = ".so";
                    loader = NativeMethods.Unix_LoadLibrary;
                    // Will fallback not exist if platform is linux.
                    baseNativePaths = new[] { baseNativePath0 }.
                        Concat(Directory.EnumerateDirectories(
                            baseNativeBasePath, $"{shortPlatform}*", SearchOption.TopDirectoryOnly).
                            Where(path => path.EndsWith(RuntimeEnvironment.RuntimeArchitecture)).
                            SelectMany(path => Directory.EnumerateDirectories(path, "native", SearchOption.TopDirectoryOnly))).
                        Concat(Directory.EnumerateDirectories(
                            $"linux-{RuntimeEnvironment.RuntimeArchitecture}", "native", SearchOption.TopDirectoryOnly)).
                        ToArray();
                    break;
            }

            PreloadNativeLibrary(log, baseNativePaths, loader);

            PreloadAssemblies(log);
        }

        private static void PreloadNativeLibrary(TaskLoggingHelper log, string[] baseNativePaths, Func<string, IntPtr> loader)
        {
            // SUPER DIRTY WORKAROUND: Will copy native library into assembly directory...
            var sourcePaths = baseNativePaths.
                SelectMany(path => Directory.EnumerateDirectories(path, "*" + NativeExtension, SearchOption.TopDirectoryOnly)).
                ToArray();
            foreach (var sourcePath in sourcePaths)
            {
                var fileName = Path.GetFileName(sourcePath);
                var destinationPath = Path.Combine(BasePath, fileName);

                // Try preloading already copied file
                var result = loader(destinationPath);

                // Success
                if (result != IntPtr.Zero)
                {
                    NativeLibraryPath = destinationPath;
                    log.LogMessage(
                        MessageImportance.High,
                        $"RelaxVersioner[{EnvironmentIdentifier}]: Native library preloaded: Path={destinationPath}");
                    return;
                }

                try
                {
                    File.Copy(sourcePath, destinationPath, true);

                    // Try preloading copied file
                    result = loader(destinationPath);

                    // Success
                    if (result != IntPtr.Zero)
                    {
                        NativeLibraryPath = destinationPath;
                        log.LogMessage(
                            MessageImportance.High,
                            $"RelaxVersioner[{EnvironmentIdentifier}]: Native library preloaded: SourcePath={sourcePath}, DestinationPath={destinationPath}");
                        return;
                    }
                }
                catch
                {
                }
            }

            var sources = string.Join(",", sourcePaths);
            log.LogWarning(
                $"RelaxVersioner[{EnvironmentIdentifier}]: Cannot preload native library: Sources=[{sources}]");
        }

        private static void PreloadAssemblies(TaskLoggingHelper log)
        {
            // Preload assemblies
            foreach (var sourcePath in Directory.EnumerateDirectories(BasePath, "*.dll", SearchOption.TopDirectoryOnly))
            {
                var assembly = Assembly.LoadFrom(sourcePath);
                if (assembly != null)
                {
                    log.LogMessage(
                        MessageImportance.High,
                        $"RelaxVersioner[{EnvironmentIdentifier}]: Assembly preloaded: Path={sourcePath}");
                }
                else
                {
                    log.LogWarning(
                        $"RelaxVersioner[{EnvironmentIdentifier}]: Cannot preload assembly: Path={sourcePath}");
                }
            }
        }
    }
}
