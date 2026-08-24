// <copyright file="Program.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.CommandLine;

namespace Foody.Gemini;

/// <summary>
/// Entry point for Foody.Gemini.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Application entry point.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>Process exit code.</returns>
    public static async Task<int> Main(string[] args)
    {
        Argument<FileInfo> pathArgument = new("path")
        {
            Description = "Path to a markdown food diary that may reference images in the same folder.",
        };

        RootCommand rootCommand = new(
            "Foody.Gemini — convert a markdown food diary (with images) into Google Health bulk intake via Gemini.")
        {
            pathArgument,
        };

        rootCommand.SetAction(async (parseResult, cancellationToken) =>
        {
            FileInfo path = parseResult.GetValue(pathArgument)!;
            return await ProcessCommandHandler.HandleAsync(path, cancellationToken).ConfigureAwait(false);
        });

        return await rootCommand.Parse(args).InvokeAsync().ConfigureAwait(false);
    }
}