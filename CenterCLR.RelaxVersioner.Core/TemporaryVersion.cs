// This is auto-generated version information attributes by RelaxVersioner.2.0.9.0, Do not edit.
// Generated date: Wed, 10 Nov 2021 14:05:13 GMT

using System.Reflection;

namespace RelaxVersioner
{
    internal static class ThisAssembly
    {
        public const string @AssemblyVersion = @"2.3.5";
        public const string @AssemblyFileVersion = @"2021.11.10.41552";
        public const string @AssemblyInformationalVersion = @"9e2409457da62511e783712a8fbd7d95e5cbfef4";
        public static class AssemblyMetadata
        {
            public const string @Date = @"Wed, 10 Nov 2021 14:05:05 GMT";
            public const string @Branch = @"devel";
            public const string @Tags = @"";
            public const string @Author = @"Kouji Matsui <k@kekyo.net>";
            public const string @Committer = @"Kouji Matsui <k@kekyo.net>";
            public const string @Message = @"Temporary downgraded self-hosted versioning.";
            public const string @Build = @"";
            public const string @Generated = @"Wed, 10 Nov 2021 14:05:13 GMT";
            public const string @TargetFramework = @"net40";
        }
    }
}

#if NET10 || NET11 || NET20 || NET30 || NET35 || NET40_CLIENT

namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    internal sealed class AssemblyMetadataAttribute : Attribute
    {
        public AssemblyMetadataAttribute(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
        public string Key { get; private set; }
        public string Value { get; private set; }
    }
}

#endif

