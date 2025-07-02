////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

namespace RelaxVersioner;

public sealed class ProcessorContext
{
    public string ProjectDirectory = null!;
    public string OutputPath = null!;
    public string Language = null!;
    public string? Namespace;
    public string? TargetFramework;
    public string? TargetFrameworkIdentity;
    public string? TargetFrameworkVersion;
    public string? TargetFrameworkProfile;
    public bool GenerateStatic;
    public string? BuildIdentifier;
    public string? PropertiesPath;
    public string TextFormat = "{versionLabel}";
    public string? ReplaceInputPath;
    public string BracketStart = "{";
    public string BracketEnd = "}";
    public bool IsDryRun;
    public bool IsQuietOnStandardOutput;
    public bool CheckWorkingDirectoryStatus;
    public string[]? NpmPrefixes;
}
