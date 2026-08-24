// <copyright file="FoodyService.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.Json;

using Foody.Lib.Internal;

namespace Foody.Lib;

/// <summary>
/// String-oriented Foody operations (no file or console I/O).
/// </summary>
public sealed class FoodyService : IAsyncDisposable
{
    private static readonly JsonSerializerOptions ParseOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private readonly GoogleHealthClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="FoodyService"/> class.
    /// </summary>
    /// <param name="options">Google Health credentials; when null, reads from environment.</param>
    /// <param name="httpClient">Optional HTTP client for Google Health API calls.</param>
    public FoodyService(GoogleHealthOptions? options = null, HttpClient? httpClient = null)
    {
        this.client = new GoogleHealthClient(options ?? GoogleHealthOptions.FromEnvironment(), httpClient);
    }

    /// <summary>
    /// Returns help text describing available Foody operations.
    /// </summary>
    /// <returns>Help text.</returns>
    public static string GetHelpText()
    {
        return """
            Foody — log food intake to Google Health API.

            Operations:
              intake <json>     Parse intake JSON and log it to Google Health API
              sample            Return a sample intake JSON document
              schema            Return the JSON Schema for intake documents
              list mealtype     List possible meal type values
              help              Show this help
            """.TrimEnd() + "\n";
    }

    /// <summary>
    /// Returns a sample intake JSON document.
    /// </summary>
    /// <returns>Sample JSON text.</returns>
    public static string GetSampleJson()
    {
        return SampleIntakeJson.Content.TrimEnd() + "\n";
    }

    /// <summary>
    /// Returns the JSON Schema for intake documents.
    /// </summary>
    /// <returns>JSON Schema text.</returns>
    public static string GetSchemaJson()
    {
        return IntakeJsonSchema.Create().TrimEnd() + "\n";
    }

    /// <summary>
    /// Returns allowed meal type values, one per line.
    /// </summary>
    /// <returns>Meal type list text.</returns>
    public static string ListMealTypes()
    {
        return string.Join("\n", MealTypes.Values) + "\n";
    }

    /// <summary>
    /// Parses intake JSON text and logs it to Google Health API.
    /// </summary>
    /// <param name="intakeJson">Intake JSON document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw JSON response body from Google Health API.</returns>
    public async Task<string> LogIntakeAsync(string intakeJson, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(intakeJson);

        FoodIntake intake;
        try
        {
            intake = JsonSerializer.Deserialize<FoodIntake>(intakeJson, ParseOptions)
                     ?? throw new InvalidOperationException("JSON root must be an object.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse intake JSON: {ex.Message}", ex);
        }

        if (string.IsNullOrWhiteSpace(intake.FoodDisplayName))
        {
            throw new InvalidOperationException("Intake JSON must include a non-empty 'foodDisplayName'.");
        }

        return await this.client.LogNutritionAsync(intake, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return this.client.DisposeAsync();
    }
}