////////////////////////////////////////////////////////////////////////////////////////
//
// RelaxVersioner - Git tag/branch based, full-automatic version generator.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mi.kekyo.net)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////////////////

using System;
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
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        
        process.Start();
        
        // Use Task.Run to wrap the synchronous WaitForExit for compatibility
        await Task.Run(() => process.WaitForExit());
        
        if (process.ExitCode != 0)
        {
            var error = process.StandardError.ReadToEnd();
            throw new InvalidOperationException($"Git command failed: git {arguments}\nError: {error}");
        }
    }

    public static async Task<string> RunGitCommandWithOutputAsync(string workingDirectory, string arguments)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        
        process.Start();
        
        // Use Task.Run to wrap the synchronous WaitForExit for compatibility
        await Task.Run(() => process.WaitForExit());
        
        if (process.ExitCode != 0)
        {
            var error = process.StandardError.ReadToEnd();
            throw new InvalidOperationException($"Git command failed: git {arguments}\nError: {error}");
        }
        
        return process.StandardOutput.ReadToEnd();
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
} 