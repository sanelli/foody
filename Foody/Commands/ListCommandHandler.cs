// <copyright file="ListCommandHandler.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Foody.Lib;

namespace Foody.Commands;

/// <summary>
/// Handles the <c>list</c> CLI command and its subcommands.
/// </summary>
internal static class ListCommandHandler
{
    /// <summary>
    /// Prints the allowed Google Health API meal type values.
    /// </summary>
    /// <returns>Process exit code (<c>0</c> on success).</returns>
    public static async Task<int> HandleMealTypeAsync()
    {
        await Console.Out.WriteAsync(FoodyService.ListMealTypes()).ConfigureAwait(false);
        return 0;
    }

    /// <summary>
    /// Prints the available schema kind identifiers.
    /// </summary>
    /// <returns>Process exit code (<c>0</c> on success).</returns>
    public static async Task<int> HandleSchemaAsync()
    {
        await Console.Out.WriteAsync(FoodyService.ListSchemaKinds()).ConfigureAwait(false);
        return 0;
    }
}