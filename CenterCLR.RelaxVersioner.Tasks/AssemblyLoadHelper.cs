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
using System.Threading;
using LibGit2Sharp;
using Microsoft.DotNet.PlatformAbstractions;

namespace CenterCLR.RelaxVersioner
{
    internal static class AssemblyLoadHelper
    {
        public static string BasePath { get; private set; }
        public static string EnvironmentIdentifier { get; private set; }

        public static string NativeRuntimeIdentifier { get; private set; }
        public static string NativePrefix { get; private set; }
        public static string NativeExtension { get; private set; }
        public static string NativeLibraryPath { get; private set; }

        static AssemblyLoadHelper()
        {
            BasePath = Path.GetDirectoryName(
                (new Uri(typeof(AssemblyLoadHelper).Assembly.CodeBase, UriKind.RelativeOrAbsolute)).LocalPath);

            var relaxVersionerVersion = typeof(AssemblyLoadHelper).Assembly.GetName().Version;
            var platformIdentifier = BasePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Last();
            var runtimeIdentifier = RuntimeEnvironment.GetRuntimeIdentifier();

            // "0.9.45/netstandard2.0/win10-x64"
            EnvironmentIdentifier = $"{relaxVersionerVersion}/{platformIdentifier}/{runtimeIdentifier}";
        }

        public static void Initialize(Logger logger)
        {
            if (NativeRuntimeIdentifier != null)
            {
                return;
            }

            var baseNativeBasePath = Path.GetFullPath(Path.Combine(BasePath, "..", "runtimes"));

            var runtimeIdentifier = RuntimeEnvironment.GetRuntimeIdentifier();
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
                    NativePrefix = string.Empty;
                    NativeExtension = ".dll";
                    baseNativePaths = new[] { baseNativePath0 };
                    loader = NativeMethods.Win32_LoadLibrary;
                    break;
                case Platform.Darwin:
                    NativePrefix = "lib";
                    NativeExtension = ".dylib";
                    baseNativePaths = new[] { baseNativePath0 };
                    loader = NativeMethods.Unix_LoadLibrary;
                    break;
                default:
                    NativePrefix = "lib";
                    NativeExtension = ".so";
                    loader = NativeMethods.Unix_LoadLibrary;
                    // Will fallback not exist if platform is linux.
                    baseNativePaths = new[] { baseNativePath0 }.
                        Concat(Directory.EnumerateDirectories(
                            baseNativeBasePath, $"{shortPlatform}*", SearchOption.TopDirectoryOnly).
                            Where(path => path.EndsWith(RuntimeEnvironment.RuntimeArchitecture)).
                            SelectMany(path => Directory.EnumerateDirectories(path, "native", SearchOption.TopDirectoryOnly))).
                        Concat(Directory.EnumerateDirectories(
                            Path.Combine(baseNativeBasePath, $"linux-{RuntimeEnvironment.RuntimeArchitecture}"), "native", SearchOption.TopDirectoryOnly)).
                        ToArray();
                    break;
            }

            PreloadNativeLibrary(logger, baseNativePaths, loader);
            PreloadAssemblies(logger);
        }

        private static void PreloadNativeLibrary(Logger logger, string[] baseNativePaths, Func<string, IntPtr> loader)
        {
            // SUPER DIRTY WORKAROUND: Will copy native library into assembly directory...
            //   Because this problem fixing is very difficult and shame .NET Core 2,
            //   I think the copy procedure is safe at user-space file system on standard nuget local repository.
            //   Related issues:
            //     https://github.com/dotnet/coreclr/issues/19654
            //     https://github.com/AArnott/Nerdbank.GitVersioning/issues/217

            // public const string Name = "git2-dd2d538";
            var gitDllName = (string)typeof(Repository).Assembly.GetTypes().
                First(type => type.FullName == "LibGit2Sharp.Core.NativeDllName").
                GetField("Name", BindingFlags.Public | BindingFlags.Static).
                GetValue(null);

            var sourcePaths = baseNativePaths.
                Where(path => Directory.Exists(path)).
                SelectMany(path => Directory.EnumerateFiles(path, $"{NativePrefix}{gitDllName}{NativeExtension}", SearchOption.TopDirectoryOnly)).
                ToArray();
            foreach (var sourcePath in sourcePaths)
            {
                var fileName = Path.GetFileName(sourcePath);
                var destinationPath = Path.Combine(BasePath, fileName);

                for (var i = 0; i < 5; i++)
                {
                    // Try preloading already copied file
                    var result = loader(destinationPath);

                    // Success
                    if (result != IntPtr.Zero)
                    {
                        NativeLibraryPath = destinationPath;
                        logger.Message(
                            LogImportance.Low,
                            "Native library preloaded: Path={0}",
                            destinationPath);
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
                            NativeLibraryPath = sourcePath;
                            logger.Message(
                                LogImportance.Low,
                                "Native library copied and preloaded: SourcePath={0}, DestinationPath={1}",
                                sourcePath,
                                destinationPath);
                            return;
                        }
                    }
                    catch
                    {
                        Thread.Sleep(200);
                    }
                }
            }

            logger.Warning(
                "Cannot preload native library: Sources=[{0}]",
                string.Join(",", sourcePaths));
        }

        private static void PreloadAssemblies(Logger logger)
        {
            // Preload assemblies
            foreach (var sourcePath in Directory.EnumerateFiles(BasePath, "*.dll", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(sourcePath);
                    if (assembly != null)
                    {
                        logger.Message(
                            LogImportance.Low,
                            "Assembly preloaded: Path={0}",
                            sourcePath);
                    }
                }
                catch (BadImageFormatException)
                {
                }
                catch
                {
                    logger.Warning(
                        "Cannot preload assembly: Path={0}",
                        sourcePath);
                }
            }
        }
    }
}
