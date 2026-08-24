// <copyright file="SchemaCommandHandler.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Foody.Commands;

/// <summary>
/// Handles the <c>schema</c> CLI command.
/// </summary>
internal static class SchemaCommandHandler
{
    /// <summary>
    /// Prints the JSON Schema for intake documents to the console.
    /// </summary>
    /// <returns>Process exit code (<c>0</c> on success).</returns>
    public static async Task<int> HandleAsync()
    {
        string schema = IntakeJsonSchema.Create().TrimEnd() + Environment.NewLine;
        await Console.Out.WriteAsync(schema).ConfigureAwait(false);
        return 0;
    }
}