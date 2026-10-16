// <copyright file="GeminiIntakeClient.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Foody.Lib;

using Google.GenAI;
using Google.GenAI.Types;

namespace Foody.Gemini;

/// <summary>
/// Calls Gemini to turn a food diary into Foody bulk-intake JSON.
/// </summary>
internal sealed class GeminiIntakeClient
{
    private readonly GeminiOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeminiIntakeClient"/> class.
    /// </summary>
    /// <param name="options">Gemini configuration.</param>
    public GeminiIntakeClient(GeminiOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        this.options = options;
    }

    /// <summary>
    /// Asks Gemini to produce bulk-intake JSON for the diary markdown and images.
    /// </summary>
    /// <param name="markdown">Diary markdown text.</param>
    /// <param name="images">Local diary images.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw model text response.</returns>
    public async Task<string> GenerateBulkIntakeJsonAsync(
        string markdown,
        IReadOnlyList<DiaryImage> images,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(markdown);
        ArgumentNullException.ThrowIfNull(images);

        if (string.IsNullOrWhiteSpace(this.options.ApiKey))
        {
            throw new InvalidOperationException(
                "Missing Gemini:ApiKey in configuration. Create an API key in Google AI Studio and set it in appsettings.json or appsettings.Local.json.");
        }

        string schema = FoodyService.GetSchemaJson(SchemaKind.BulkIntake);
        List<Part> parts =
        [
            Part.FromText(
                """
                You are converting a personal food diary into Foody bulk-intake JSON.

                Return ONLY a JSON that validates against the schema below.
                Do not include markdown fences or any explanation.
                Infer mealType, times, energy, and macros from the diary text and images when possible.
                Use ISO-8601 UTC timestamps ending with Z.
                If a time is unknown, estimate reasonable start/end times for that meal on the diary day.
                Keep foodDisplayName concise and human readable.
                Limit the number items to at most 10000 items.

                JSON Schema:
                """ + schema),
            Part.FromText("Food diary markdown:\n" + markdown),
        ];

        foreach (DiaryImage image in images)
        {
            cancellationToken.ThrowIfCancellationRequested();
            (byte[] bytes, string mimeType) = await ImageCompressor.CompressAsync(image.Path, cancellationToken)
                .ConfigureAwait(false);
            parts.Add(Part.FromText($"Attached image referenced as '{image.Reference}' (path: {Path.GetFileName(image.Path)})."));
            parts.Add(Part.FromBytes(bytes, mimeType));
        }

        using Client client = new(apiKey: this.options.ApiKey);
        GenerateContentConfig config = new()
        {
            ResponseMimeType = "application/json",
            Temperature = 0.2,
        };

        GenerateContentResponse response = await client.Models.GenerateContentAsync(
            model: this.options.Model,
            contents: new Content
            {
                Role = "user",
                Parts = parts,
            },
            config: config,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        string? text = response.Candidates?
            .SelectMany(static candidate => candidate.Content?.Parts ?? [])
            .Select(static part => part.Text)
            .FirstOrDefault(static value => !string.IsNullOrWhiteSpace(value));

        if (string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(response.Text))
        {
            text = response.Text;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException("Gemini returned an empty response.");
        }

        return text;
    }
}