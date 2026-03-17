////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RelaxVersioner;

public sealed class ResolvePackWorkspaceTask : Task
{
    [Required]
    public string? RootPath
    {
        get;
        set;
    }

    public string? TargetFramework
    {
        get;
        set;
    }

    [Output]
    public string? RequestKey
    {
        get;
        private set;
    }

    [Output]
    public string? WorkspacePath
    {
        get;
        private set;
    }

    public override bool Execute()
    {
        if (string.IsNullOrWhiteSpace(this.RootPath))
        {
            this.Log.LogError("RelaxVersioner.ResolvePackWorkspaceTask: Required root path.");
            return false;
        }

        var requestKey =
            this.GetBuildRequestKey() ??
            $"req_{Guid.NewGuid():N}";
        var targetFramework =
            SanitizePathSegment(this.TargetFramework) switch
            {
                "" => "_outer",
                var value => value,
            };

        this.RequestKey = requestKey;
        this.WorkspacePath = Path.Combine(
            Path.GetFullPath(this.RootPath),
            "RelaxVersioner_Pack",
            targetFramework,
            requestKey);

        this.Log.LogMessage(
            $"RelaxVersioner.ResolvePackWorkspaceTask: Resolved pack workspace, RequestKey={this.RequestKey}, Path={this.WorkspacePath}");
        return true;
    }

    private string? GetBuildRequestKey()
    {
        var buildRequestEntry = this.BuildEngine?.GetField("_requestEntry");
        var buildRequest =
            buildRequestEntry?.GetProperty("BuildRequest") ??
            buildRequestEntry?.GetProperty("Request");

        var requestKey =
            GetPropertyString(buildRequest, "GlobalRequestId") ??
            GetPropertyString(buildRequest, "RequestId") ??
            GetPropertyString(buildRequest, "NodeRequestId");

        if (!string.IsNullOrWhiteSpace(requestKey))
        {
            return $"req_{SanitizePathSegment(requestKey)}";
        }

        var requestConfiguration = buildRequestEntry?.GetProperty("RequestConfiguration");
        requestKey =
            GetPropertyString(requestConfiguration, "ConfigurationId") ??
            GetPropertyString(requestConfiguration, "GlobalConfigurationId");

        return string.IsNullOrWhiteSpace(requestKey) ?
            null :
            $"cfg_{SanitizePathSegment(requestKey)}";
    }

    private static string? GetPropertyString(object? instance, string name) =>
        instance?.GetProperty(name)?.ToString();

    private static string SanitizePathSegment(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var buffer = value!.ToCharArray();

        for (var index = 0; index < buffer.Length; index++)
        {
            var ch = buffer[index];
            if ((ch == Path.DirectorySeparatorChar) ||
                (ch == Path.AltDirectorySeparatorChar) ||
                (Array.IndexOf(invalidChars, ch) >= 0))
            {
                buffer[index] = '_';
            }
        }

        return new string(buffer);
    }
}
