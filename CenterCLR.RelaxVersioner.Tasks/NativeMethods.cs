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
using System.Runtime.InteropServices;

namespace CenterCLR.RelaxVersioner
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", EntryPoint = "AddDllDirectory", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool AddDllDirectory(string path);

        public static bool Win32_AddDllDirectory(string path)
        {
            try
            {
                return AddDllDirectory(path);
            }
            catch
            {
                return false;
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Win32_LoadLibrary(string dllPath);


        [Flags]
        private enum RTLD : int
        {
            LAZY = 0x00001,        /* Lazy function call binding.  */
            NOW = 0x00002,         /* Immediate function call binding.  */
            NOLOAD = 0x00004,      /* Do not load the object.  */
            DEEPBIND = 0x00008,    /* Use deep binding.  */
            GLOBAL = 0x00100,
            LOCAL = 0x00000
        };

        [DllImport("dl", EntryPoint = "dlopen", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr dlopen(string dllPath, RTLD flags);

        public static IntPtr Unix_LoadLibrary(string dllPath) =>
            (dlopen(dllPath, RTLD.NOW | RTLD.GLOBAL) is IntPtr result && (result != IntPtr.Zero)) ? result : dlopen(dllPath, RTLD.NOW);
    }
}
