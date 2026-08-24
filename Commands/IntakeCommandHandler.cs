// <copyright file="IntakeCommandHandler.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.Json;

using Foody.Models;
using Foody.Services;

namespace Foody.Commands;

/// <summary>
/// Handles the <c>intake</c> CLI command.
/// </summary>
internal static class IntakeCommandHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    /// <summary>
    /// Parses the intake JSON file and logs it to Google Health API.
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

        FoodIntake intake;
        try
        {
            FileStream stream = path.OpenRead();
            await using (stream.ConfigureAwait(false))
            {
                intake = await JsonSerializer.DeserializeAsync<FoodIntake>(stream, JsonOptions, cancellationToken)
                         .ConfigureAwait(false)
                         ?? throw new InvalidOperationException("JSON root must be an object.");
            }
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            await Console.Error.WriteLineAsync($"Failed to parse intake JSON: {ex.Message}").ConfigureAwait(false);
            return 1;
        }

        if (string.IsNullOrWhiteSpace(intake.FoodDisplayName))
        {
            await Console.Error.WriteLineAsync("Intake JSON must include a non-empty 'foodDisplayName'.").ConfigureAwait(false);
            return 1;
        }

        GoogleHealthOptions options = GoogleHealthOptions.FromEnvironment();
        GoogleHealthClient client = new(options);
        await using (client.ConfigureAwait(false))
        {
            try
            {
                await Console.Out.WriteLineAsync($"Logging '{intake.FoodDisplayName}' to Google Health API...").ConfigureAwait(false);
                string response = await client.LogNutritionAsync(intake, cancellationToken).ConfigureAwait(false);
                await Console.Out.WriteLineAsync("Intake logged successfully.").ConfigureAwait(false);
                await Console.Out.WriteLineAsync(response).ConfigureAwait(false);
                return 0;
            }
            catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or IOException)
            {
                await Console.Error.WriteLineAsync($"Failed to log intake: {ex.Message}").ConfigureAwait(false);
                return 1;
            }
        }
    }
}