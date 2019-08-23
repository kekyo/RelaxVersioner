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
using Microsoft.DotNet.PlatformAbstractions;

namespace CenterCLR.RelaxVersioner.Loader
{
    internal static class AssemblyLoadHelper
    {
        public static readonly string AssemblyPath =
            (new Uri(typeof(AssemblyLoadHelper).Assembly.CodeBase, UriKind.RelativeOrAbsolute)).LocalPath;
        public static readonly string BasePath =
            Path.GetDirectoryName(AssemblyPath);
        public static readonly string BaseNativePath;

        static AssemblyLoadHelper()
        {
            var baseNativePath = Path.GetFullPath(Path.Combine(BasePath, "..", "runtimes"));

            var id = RuntimeEnvironment.GetRuntimeIdentifier().Replace("-aot", string.Empty);
            var ids = id.Split('-');
            var platform0 = ids[0].Split('.')[0].Trim('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            var platform = string.Join("-", new[] { platform0 }.Concat(ids.Skip(1).Take(ids.Length - 2)));
            BaseNativePath = Path.Combine(baseNativePath, $"{platform}-{RuntimeEnvironment.RuntimeArchitecture}", "native");

            // Will fallback not exist if platform is linux.
            if (!Directory.Exists(BaseNativePath) && (RuntimeEnvironment.OperatingSystemPlatform == Platform.Linux))
            {
                BaseNativePath = Path.Combine(baseNativePath, $"linux-{RuntimeEnvironment.RuntimeArchitecture}", "native");
            }

            // Preload assemblies same as the directory.
            foreach (var path in Directory.EnumerateFiles(BasePath, "*.dll", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    Assembly.LoadFrom(path);
                }
                catch
                {
                }
            }
        }
    }
}
