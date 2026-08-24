// <copyright file="Program.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.CommandLine;

using Foody.Commands;
using Foody.Lib;

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

        Argument<FileInfo> bulkIntakePathArgument = new("path")
        {
            Description = "Path to a JSON array file of intake objects to import (max 10000 items).",
        };

        Command bulkIntakeCommand = new("bulk-intake", "Import multiple intake JSON objects via Google Health API.")
        {
            bulkIntakePathArgument,
        };

        bulkIntakeCommand.SetAction(async (parseResult, cancellationToken) =>
        {
            FileInfo path = parseResult.GetValue(bulkIntakePathArgument)!;
            return await BulkIntakeCommandHandler.HandleAsync(path, cancellationToken).ConfigureAwait(false);
        });

        Command sampleCommand = new("sample", "Print a sample intake JSON document to the console.");
        sampleCommand.SetAction(async (_, _) =>
        {
            return await SampleCommandHandler.HandleAsync().ConfigureAwait(false);
        });

        Command schemaIntakeCommand = new(SchemaKind.Intake, "Print the JSON Schema for a single intake object.");
        schemaIntakeCommand.SetAction(async (_, _) =>
        {
            return await SchemaCommandHandler.HandleAsync(SchemaKind.Intake).ConfigureAwait(false);
        });

        Command schemaBulkIntakeCommand = new(SchemaKind.BulkIntake, "Print the JSON Schema for a bulk-intake array.");
        schemaBulkIntakeCommand.SetAction(async (_, _) =>
        {
            return await SchemaCommandHandler.HandleAsync(SchemaKind.BulkIntake).ConfigureAwait(false);
        });

        Command schemaCommand = new("schema", "Print a JSON Schema (intake or bulk-intake).")
        {
            schemaIntakeCommand,
            schemaBulkIntakeCommand,
        };

        Command mealTypeCommand = new("mealtype", "List possible meal type values for intake JSON.");
        mealTypeCommand.SetAction(async (_, _) =>
        {
            return await ListCommandHandler.HandleMealTypeAsync().ConfigureAwait(false);
        });

        Command listSchemaCommand = new("schema", "List available schema kinds.");
        listSchemaCommand.SetAction(async (_, _) =>
        {
            return await ListCommandHandler.HandleSchemaAsync().ConfigureAwait(false);
        });

        Command listCommand = new("list", "List allowed values for intake fields.")
        {
            mealTypeCommand,
            listSchemaCommand,
        };

        Command helpCommand = new("help", "Show help and usage information.");
        helpCommand.SetAction(async (_, _) =>
        {
            return await HelpCommandHandler.HandleAsync().ConfigureAwait(false);
        });

        RootCommand rootCommand = new("Foody — log food intake to Google Health API.")
        {
            intakeCommand,
            bulkIntakeCommand,
            sampleCommand,
            schemaCommand,
            listCommand,
            helpCommand,
        };

        return await rootCommand.Parse(args).InvokeAsync().ConfigureAwait(false);
    }
}