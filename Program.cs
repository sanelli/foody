// <copyright file="Program.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.CommandLine;

using Foody.Commands;

namespace Foody;

/// <summary>
/// Entry point for the Foody console application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Application entry point.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>A task that resolves to the process exit code.</returns>
    public static async Task<int> Main(string[] args)
    {
        Argument<FileInfo> intakePathArgument = new("path")
        {
            Description = "Path to a JSON file describing the food intake to log.",
        };

        Command intakeCommand = new("intake", "Parse an intake JSON file and log it to Google Health API.")
        {
            intakePathArgument,
        };

        intakeCommand.SetAction(async (parseResult, cancellationToken) =>
        {
            FileInfo path = parseResult.GetValue(intakePathArgument)!;
            return await IntakeCommandHandler.HandleAsync(path, cancellationToken).ConfigureAwait(false);
        });

        Command sampleCommand = new("sample", "Print a sample intake JSON document to the console.");
        sampleCommand.SetAction(async (_, _) =>
        {
            return await SampleCommandHandler.HandleAsync().ConfigureAwait(false);
        });

        Command schemaCommand = new("schema", "Print the JSON Schema for intake documents accepted by intake.");
        schemaCommand.SetAction(async (_, _) =>
        {
            return await SchemaCommandHandler.HandleAsync().ConfigureAwait(false);
        });

        Command mealTypeCommand = new("mealtype", "List possible meal type values for intake JSON.");
        mealTypeCommand.SetAction(async (_, _) =>
        {
            return await ListCommandHandler.HandleMealTypeAsync().ConfigureAwait(false);
        });

        Command listCommand = new("list", "List allowed values for intake fields.")
        {
            mealTypeCommand,
        };

        RootCommand rootCommand = new("Foody — log food intake to Google Health API.")
        {
            intakeCommand,
            sampleCommand,
            schemaCommand,
            listCommand,
        };

        Command helpCommand = new("help", "Show help and usage information.");
        helpCommand.SetAction(async (_, cancellationToken) =>
        {
            return await rootCommand.Parse(["--help"]).InvokeAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        });
        rootCommand.Subcommands.Add(helpCommand);

        return await rootCommand.Parse(args).InvokeAsync().ConfigureAwait(false);
    }
}