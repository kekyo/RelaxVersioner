////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RelaxVersioner;

public static class TestUtilities
{
    // Makes safer exactly utf8 content reading.
    // System.IO.File.ReadAllText() is broken in end of file lines, why....
    public static async Task<string> ReadAllTextAsync(string path)
    {
        using var s = File.OpenRead(path);
        var tr = new StreamReader(s, Utilities.UTF8, true);
        return await tr.ReadToEndAsync();
    }

    public static async Task RunGitCommandAsync(string workingDirectory, string arguments)
    {
        try
        {
            await RunProcessCoreAsync("git", workingDirectory, arguments, null);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"Git command failed: git {arguments}\nError: {ex.Message}", ex);
        }
    }

    public static async Task<string> RunGitCommandWithOutputAsync(string workingDirectory, string arguments)
    {
        try
        {
            var (_, output, _) = await RunProcessCoreAsync("git", workingDirectory, arguments, null);
            return output;
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"Git command failed: git {arguments}\nError: {ex.Message}", ex);
        }
    }

    public static async Task RunCommandAsync(
        string fileName,
        string workingDirectory,
        string arguments,
        IReadOnlyDictionary<string, string?>? environmentVariables = null) =>
        await RunProcessCoreAsync(fileName, workingDirectory, arguments, environmentVariables);

    public static async Task<string> RunCommandWithOutputAsync(
        string fileName,
        string workingDirectory,
        string arguments,
        IReadOnlyDictionary<string, string?>? environmentVariables = null)
    {
        var (_, output, _) = await RunProcessCoreAsync(
            fileName,
            workingDirectory,
            arguments,
            environmentVariables);
        return output;
    }

    public static async Task InitializeGitRepositoryWithMainBranch(string workingDirectory)
    {
        try
        {
            // Try modern Git init with initial branch specification
            await RunGitCommandAsync(workingDirectory, "init --initial-branch=main");
        }
        catch
        {
            // Fallback for older Git versions
            await RunGitCommandAsync(workingDirectory, "init");
            await RunGitCommandAsync(workingDirectory, "config init.defaultBranch main");
            // Check if we need to rename the branch
            try
            {
                var currentBranch = await RunGitCommandWithOutputAsync(workingDirectory, "branch --show-current");
                if (currentBranch.Trim() != "main")
                {
                    await RunGitCommandAsync(workingDirectory, "branch -m main");
                }
            }
            catch
            {
                // If no commits yet, the branch rename will happen after first commit
            }
        }
    }

    private static async Task<(int ExitCode, string StandardOutput, string StandardError)> RunProcessCoreAsync(
        string fileName,
        string workingDirectory,
        string arguments,
        IReadOnlyDictionary<string, string?>? environmentVariables)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        if (environmentVariables != null)
        {
            foreach (var (key, value) in environmentVariables)
            {
                process.StartInfo.Environment[key] = value ?? string.Empty;
            }
        }

        process.Start();

        var standardOutputTask = process.StandardOutput.ReadToEndAsync();
        var standardErrorTask = process.StandardError.ReadToEndAsync();

        await Task.Run(() => process.WaitForExit());

        var standardOutput = await standardOutputTask;
        var standardError = await standardErrorTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Command failed: {fileName} {arguments}\nError: {standardError}");
        }

        return (process.ExitCode, standardOutput, standardError);
    }
}
