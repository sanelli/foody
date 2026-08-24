// <copyright file="HelpCommandHandler.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Foody.Lib;

namespace Foody.Commands;

/// <summary>
/// Handles the <c>help</c> CLI command.
/// </summary>
internal static class HelpCommandHandler
{
    /// <summary>
    /// Prints shared Foody help text to the console.
    /// </summary>
    /// <returns>Process exit code (<c>0</c> on success).</returns>
    public static async Task<int> HandleAsync()
    {
        await Console.Out.WriteAsync(FoodyService.GetHelpText()).ConfigureAwait(false);
        return 0;
    }
}