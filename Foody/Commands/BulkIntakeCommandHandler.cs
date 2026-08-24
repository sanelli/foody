// <copyright file="BulkIntakeCommandHandler.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Foody.Lib;

namespace Foody.Commands;

/// <summary>
/// Handles the <c>bulk-intake</c> CLI command.
/// </summary>
internal static class BulkIntakeCommandHandler
{
    /// <summary>
    /// Reads a bulk intake JSON array file and imports it via <see cref="FoodyService"/>.
    /// </summary>
    /// <param name="path">Path to the bulk intake JSON file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Process exit code (<c>0</c> on success).</returns>
    public static async Task<int> HandleAsync(FileInfo path, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (!path.Exists)
        {
            await Console.Error.WriteLineAsync($"File not found: {path.FullName}").ConfigureAwait(false);
            return 1;
        }

        string bulkIntakeJson;
        try
        {
            bulkIntakeJson = await File.ReadAllTextAsync(path.FullName, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            await Console.Error.WriteLineAsync($"Failed to read bulk intake file: {ex.Message}").ConfigureAwait(false);
            return 1;
        }

        FoodyService service = new();
        await using (service.ConfigureAwait(false))
        {
            try
            {
                await Console.Out.WriteLineAsync("Importing bulk intake to Google Health API...").ConfigureAwait(false);
                string response = await service.LogBulkIntakeAsync(bulkIntakeJson, cancellationToken).ConfigureAwait(false);
                await Console.Out.WriteLineAsync("Bulk intake imported successfully.").ConfigureAwait(false);
                await Console.Out.WriteLineAsync(response).ConfigureAwait(false);
                return 0;
            }
            catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or ArgumentException)
            {
                await Console.Error.WriteLineAsync($"Failed to import bulk intake: {ex.Message}").ConfigureAwait(false);
                return 1;
            }
        }
    }
}