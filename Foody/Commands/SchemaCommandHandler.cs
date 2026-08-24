// <copyright file="SchemaCommandHandler.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Foody.Lib;

namespace Foody.Commands;

/// <summary>
/// Handles the <c>schema</c> CLI command.
/// </summary>
internal static class SchemaCommandHandler
{
    /// <summary>
    /// Prints the JSON Schema for the selected kind to the console.
    /// </summary>
    /// <param name="kind">Schema kind (<c>intake</c> or <c>bulk-intake</c>).</param>
    /// <returns>Process exit code (<c>0</c> on success).</returns>
    public static async Task<int> HandleAsync(string kind)
    {
        try
        {
            await Console.Out.WriteAsync(FoodyService.GetSchemaJson(kind)).ConfigureAwait(false);
            return 0;
        }
        catch (ArgumentException ex)
        {
            await Console.Error.WriteLineAsync(ex.Message).ConfigureAwait(false);
            return 1;
        }
    }
}