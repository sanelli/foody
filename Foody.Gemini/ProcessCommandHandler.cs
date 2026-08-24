// <copyright file="ProcessCommandHandler.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Foody.Lib;

namespace Foody.Gemini;

/// <summary>
/// Handles converting a markdown diary into Google Health bulk intake.
/// </summary>
internal static class ProcessCommandHandler
{
    /// <summary>
    /// Reads a markdown diary, asks Gemini for bulk-intake JSON, and uploads it.
    /// </summary>
    /// <param name="path">Path to the markdown diary.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Process exit code.</returns>
    public static async Task<int> HandleAsync(FileInfo path, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(path);

        try
        {
            (string markdown, IReadOnlyList<DiaryImage> images) =
                await MarkdownDiaryReader.ReadAsync(path.FullName).ConfigureAwait(false);

            await Console.Out.WriteLineAsync(
                $"Loaded diary '{path.FullName}' with {images.Count} local image reference(s).").ConfigureAwait(false);

            GeminiIntakeClient gemini = new(AppConfiguration.Gemini);
            string rawResponse = await gemini.GenerateBulkIntakeJsonAsync(markdown, images, cancellationToken)
                .ConfigureAwait(false);
            string bulkJson = JsonResponseExtractor.ExtractJsonArray(rawResponse);

            await Console.Out.WriteLineAsync("Gemini produced bulk-intake JSON. Uploading via Foody.Lib...").ConfigureAwait(false);
            await Console.Out.WriteLineAsync(bulkJson).ConfigureAwait(false);

            FoodyService service = new(AppConfiguration.GoogleHealth);
            await using (service.ConfigureAwait(false))
            {
                string apiResponse = await service.LogBulkIntakeAsync(bulkJson, cancellationToken).ConfigureAwait(false);
                await Console.Out.WriteLineAsync("Bulk intake uploaded successfully.").ConfigureAwait(false);
                await Console.Out.WriteLineAsync(apiResponse).ConfigureAwait(false);
            }

            return 0;
        }
        catch (Exception ex) when (ex is FileNotFoundException
                                       or InvalidOperationException
                                       or HttpRequestException
                                       or ArgumentException
                                       or IOException
                                       or UnauthorizedAccessException)
        {
            await Console.Error.WriteLineAsync($"Failed to process diary: {ex.Message}").ConfigureAwait(false);
            return 1;
        }
    }
}