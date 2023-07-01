////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version information inserter.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.IO.Compression;
using VerifyTests;

namespace RelaxVersioner;

public sealed class TestsSetUp
{
    public static readonly string BasePath =
        Path.Combine("tests", $"{DateTime.Now:yyyyMMdd_HHmmss}");

    static TestsSetUp()
    {
        VerifierSettings.DontScrubDateTimes();

        if (!Directory.Exists(BasePath))
        {
            foreach (var path in Directory.EnumerateFiles(
                "artifacts", "*.zip", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                var targetBasePath = Path.Combine(BasePath, fileName);

                try
                {
                    Directory.CreateDirectory(targetBasePath);
                }
                catch
                {
                }

                ZipFile.ExtractToDirectory(path, targetBasePath);
            }
        }
    }
}
