﻿////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RelaxVersioner;

internal static class ResultWriter
{
    private static string ToString(object? value) =>
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

        Processor.WriteSafeTransacted(path, document.Save);
    }
}
