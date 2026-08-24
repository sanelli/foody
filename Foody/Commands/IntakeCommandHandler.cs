// <copyright file="IntakeCommandHandler.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Foody.Lib;

namespace Foody.Commands;

/// <summary>
/// Handles the <c>intake</c> CLI command.
/// </summary>
internal static class IntakeCommandHandler
{
    /// <summary>
    /// Reads an intake JSON file and logs it via <see cref="FoodyService"/>.
    /// </summary>
    /// <param name="path">Path to the intake JSON file.</param>
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

        string intakeJson;
        try
        {
            intakeJson = await File.ReadAllTextAsync(path.FullName, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            await Console.Error.WriteLineAsync($"Failed to read intake file: {ex.Message}").ConfigureAwait(false);
            return 1;
        }

        FoodyService service = new(AppConfiguration.GoogleHealth);
        await using (service.ConfigureAwait(false))
        {
            try
            {
                await Console.Out.WriteLineAsync("Logging intake to Google Health API...").ConfigureAwait(false);
                string response = await service.LogIntakeAsync(intakeJson, cancellationToken).ConfigureAwait(false);
                await Console.Out.WriteLineAsync("Intake logged successfully.").ConfigureAwait(false);
                await Console.Out.WriteLineAsync(response).ConfigureAwait(false);
                return 0;
            }
            catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException)
            {
                await Console.Error.WriteLineAsync($"Failed to log intake: {ex.Message}").ConfigureAwait(false);
                return 1;
            }
        }
    }
}