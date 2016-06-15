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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CenterCLR.RelaxVersioner
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var solutionDirectory = args[0];
                var projectDirectory = args[1];
                var targetPath = args[2];
                var targetFrameworkVersion = Utilities.GetFrameworkVersionNumber(args[3]);
                var targetFrameworkProfile = args[4];
                var language = args[5];

                var combineDefinitions = new List<string>();

                var result = Executor.Execute(
                    solutionDirectory,
                    projectDirectory,
                    targetPath,
                    targetFrameworkVersion,
                    targetFrameworkProfile,
                    language,
                    combineDefinitions,
                    Console.WriteLine);
                
                // TODO: Executable cannot handle output parameters.
                Debug.Assert(combineDefinitions.Count == 0);

                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("RelaxVersioner: " + ex.Message);
                return Marshal.GetHRForException(ex);
            }
        }
    }
}
