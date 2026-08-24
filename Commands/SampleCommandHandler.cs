// <copyright file="SampleCommandHandler.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Foody.Commands;

/// <summary>
/// Handles the <c>sample</c> CLI command.
/// </summary>
internal static class SampleCommandHandler
{
    /// <summary>
    /// Writes sample intake JSON to the console.
    /// </summary>
    /// <returns>Process exit code (<c>0</c> on success).</returns>
    public static async Task<int> HandleAsync()
    {
        string content = SampleIntakeJson.Content.TrimEnd() + Environment.NewLine;
        await Console.Out.WriteAsync(content).ConfigureAwait(false);
        return 0;
    }
}