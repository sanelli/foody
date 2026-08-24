// <copyright file="FoodyService.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;
using System.Text.Json;

using Foody.Lib.Internal;

namespace Foody.Lib;

/// <summary>
/// String-oriented Foody operations (no file or console I/O).
/// </summary>
public sealed class FoodyService : IAsyncDisposable
{
    /// <summary>
    /// Maximum number of intake items accepted by <see cref="LogBulkIntakeAsync"/>.
    /// Matches the Google Health API import limit.
    /// </summary>
    public const int MaxBulkIntakeCount = BulkIntakeLimits.MaxCount;

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
              intake <json>              Parse intake JSON and log it to Google Health API
              bulk-intake <json-array>   Import multiple intake JSON objects (max 10000)
              sample                     Return a sample intake JSON document
              schema intake|bulk-intake  Return the JSON Schema for the selected payload
              list mealtype              List possible meal type values
              list schema                List available schema kinds
              help                       Show this help
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
    /// Returns the JSON Schema for the given kind.
    /// </summary>
    /// <param name="kind">Schema kind (<see cref="SchemaKind.Intake"/> or <see cref="SchemaKind.BulkIntake"/>).</param>
    /// <returns>JSON Schema text.</returns>
    public static string GetSchemaJson(string kind)
    {
        return IntakeJsonSchema.Create(kind).TrimEnd() + "\n";
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
    /// Returns available schema kind identifiers, one per line.
    /// </summary>
    /// <returns>Schema kind list text.</returns>
    public static string ListSchemaKinds()
    {
        return string.Join("\n", SchemaKind.Values) + "\n";
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

        FoodIntake intake = ParseIntakeObject(intakeJson);
        ValidateIntake(intake, index: null);
        return await this.client.LogNutritionAsync(intake, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Parses a JSON array of intake objects and imports them via Google Health API.
    /// </summary>
    /// <param name="bulkIntakeJson">JSON array of intake documents.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw JSON response body from Google Health API.</returns>
    public async Task<string> LogBulkIntakeAsync(string bulkIntakeJson, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bulkIntakeJson);

        List<FoodIntake> intakes;
        try
        {
            intakes = JsonSerializer.Deserialize<List<FoodIntake>>(bulkIntakeJson, ParseOptions)
                      ?? throw new InvalidOperationException("JSON root must be an array.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse bulk intake JSON: {ex.Message}", ex);
        }

        if (intakes.Count == 0)
        {
            throw new InvalidOperationException("Bulk intake JSON array must contain at least one item.");
        }

        if (intakes.Count > MaxBulkIntakeCount)
        {
            throw new InvalidOperationException(
                $"Bulk intake exceeds the maximum of {MaxBulkIntakeCount.ToString(CultureInfo.InvariantCulture)} items " +
                $"(received {intakes.Count.ToString(CultureInfo.InvariantCulture)}).");
        }

        for (int i = 0; i < intakes.Count; i++)
        {
            ValidateIntake(intakes[i], i);
        }

        return await this.client.ImportNutritionAsync(intakes, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return this.client.DisposeAsync();
    }

    private static FoodIntake ParseIntakeObject(string intakeJson)
    {
        try
        {
            return JsonSerializer.Deserialize<FoodIntake>(intakeJson, ParseOptions)
                   ?? throw new InvalidOperationException("JSON root must be an object.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse intake JSON: {ex.Message}", ex);
        }
    }

    private static void ValidateIntake(FoodIntake intake, int? index)
    {
        if (string.IsNullOrWhiteSpace(intake.FoodDisplayName))
        {
            string prefix = index is { } i
                ? $"Bulk intake item[{i.ToString(CultureInfo.InvariantCulture)}] "
                : "Intake JSON ";
            throw new InvalidOperationException(prefix + "must include a non-empty 'foodDisplayName'.");
        }
    }
}