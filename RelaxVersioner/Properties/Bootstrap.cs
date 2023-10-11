////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version information inserter.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

#if BOOTSTRAP
using System.Reflection;

[assembly: AssemblyVersion("0.0.1")]
[assembly: AssemblyFileVersion("2023.10.1.0")]
[assembly: AssemblyInformationalVersion("0.0.1-bootstrap")]

namespace RelaxVersioner
{
    internal static class ThisAssembly
    {
        public const string @AssemblyVersion = @"0.0.1";
        public const string @AssemblyFileVersion = @"2023.10.1.0";
        public const string @AssemblyInformationalVersion = @"0.0.1-bootstrap";
        public static class AssemblyMetadata
        {
            public const string @CommitId = @"bootstrap";
            public const string @TargetFrameworkMoniker = @"bootstrap";
        }
    }
}
#endif
