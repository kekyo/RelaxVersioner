/////////////////////////////////////////////////////////////////////////////////////////////////
//
// CenterCLR.RelaxVersioner - Easy-usage, Git-based, auto-generate version informations toolset.
// Copyright (c) 2016-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
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
using System.Xml.Linq;

namespace CenterCLR.RelaxVersioner
{
    internal static class ResultWriter
    {
        private static string ToString(object value) =>
            (value is Array array) ?
                string.Join(",", array.Cast<object>().Select(ToString)):
                value?.ToString() ?? string.Empty;

        public static void Write(string path, object result)
        {
            var type = result.GetType();
            var document = new XElement(
                type.Name,
                type.GetFields().
                    Select(field => new XElement(field.Name, ToString(field.GetValue(result)))));

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                document.Save(fs);
                fs.Flush();
            }
        }
    }
}
