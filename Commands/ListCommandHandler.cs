// <copyright file="ListCommandHandler.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Foody.Models;

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
        foreach (string mealType in MealTypes.Values)
        {
            await Console.Out.WriteLineAsync(mealType).ConfigureAwait(false);
        }

        return 0;
    }
}