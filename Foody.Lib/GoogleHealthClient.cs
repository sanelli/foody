// <copyright file="GoogleHealthClient.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using Foody.Lib.Internal;

namespace Foody.Lib;

/// <summary>
/// Minimal client for Google Health API nutrition-log create and import.
/// Auth/token refresh is intentionally stubbed for scaffolding.
/// </summary>
internal sealed class GoogleHealthClient : IAsyncDisposable
{
    /// <summary>
    /// Endpoint used to create a single nutrition-log data point.
    /// </summary>
    public const string NutritionLogEndpoint =
        "https://health.googleapis.com/v4/users/me/dataTypes/nutrition-log/dataPoints";

    /// <summary>
    /// Endpoint used to import a batch of nutrition-log data points.
    /// </summary>
    public const string NutritionLogImportEndpoint =
        "https://health.googleapis.com/v4/users/me/dataTypes/nutrition-log/dataPoints:import";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly HttpClient http;
    private readonly GoogleHealthOptions options;
    private readonly bool ownsHttp;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleHealthClient"/> class.
    /// </summary>
    /// <param name="options">Google Health API credentials and settings.</param>
    /// <param name="httpClient">Optional HTTP client; when omitted, one is created and owned.</param>
    public GoogleHealthClient(GoogleHealthOptions options, HttpClient? httpClient = null)
    {
        this.options = options;
        this.ownsHttp = httpClient is null;
        this.http = httpClient ?? new HttpClient();
    }

    /// <summary>
    /// Maps a local intake payload to a Google Health nutrition-log request and posts it.
    /// </summary>
    /// <param name="intake">Local intake payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw JSON response body from Google Health API.</returns>
    public Task<string> LogNutritionAsync(
        FoodIntake intake,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(intake);
        this.EnsureAccessToken();

        NutritionLogDataPointRequest requestBody = MapToApiRequest(intake);
        return this.SendJsonAsync(HttpMethod.Post, NutritionLogEndpoint, requestBody, cancellationToken);
    }

    /// <summary>
    /// Imports a batch of nutrition-log data points.
    /// </summary>
    /// <param name="intakes">Local intake payloads.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Raw JSON response body from Google Health API.</returns>
    public Task<string> ImportNutritionAsync(
        IReadOnlyList<FoodIntake> intakes,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(intakes);
        this.EnsureAccessToken();

        List<NutritionLogDataPointRequest> dataPoints = new(intakes.Count);
        foreach (FoodIntake intake in intakes)
        {
            dataPoints.Add(MapToApiRequest(intake));
        }

        ImportDataPointsRequest requestBody = new()
        {
            InlineSource = new ImportInlineSource
            {
                DataPoints = dataPoints,
            },
        };

        return this.SendJsonAsync(HttpMethod.Post, NutritionLogImportEndpoint, requestBody, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        if (this.ownsHttp)
        {
            this.http.Dispose();
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Maps a <see cref="FoodIntake"/> to the Google Health API data point body.
    /// </summary>
    /// <param name="intake">Local intake payload.</param>
    /// <returns>API data point body.</returns>
    internal static NutritionLogDataPointRequest MapToApiRequest(FoodIntake intake)
    {
        DateTimeOffset start = intake.StartTime ?? DateTimeOffset.UtcNow;
        DateTimeOffset end = intake.EndTime ?? start;
        CultureInfo culture = CultureInfo.InvariantCulture;

        return new NutritionLogDataPointRequest
        {
            NutritionLog = new NutritionLog
            {
                Interval = new TimeInterval
                {
                    StartTime = start.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", culture),
                    EndTime = end.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", culture),
                },
                FoodDisplayName = intake.FoodDisplayName,
                MealType = intake.MealType,
                Energy = intake.EnergyKcal is { } kcal ? new EnergyQuantity { Kcal = kcal } : null,
                TotalCarbohydrate = intake.TotalCarbohydrateGrams is { } carbs
                    ? new MassQuantity { Grams = carbs }
                    : null,
                TotalFat = intake.TotalFatGrams is { } fat
                    ? new MassQuantity { Grams = fat }
                    : null,
                Protein = intake.TotalProteinGrams is { } protein
                    ? new MassQuantity { Grams = protein }
                    : null,
            },
        };
    }

    private void EnsureAccessToken()
    {
        if (string.IsNullOrWhiteSpace(this.options.AccessToken))
        {
            throw new InvalidOperationException(
                "Missing FOODY_GOOGLE_ACCESS_TOKEN. Google Health API calls require a user OAuth access token " +
                $"with scope {GoogleHealthOptions.NutritionWriteOnlyScope}. " +
                "Client ID/secret and refresh token are also needed to obtain/refresh tokens (not configured yet).");
        }
    }

    private async Task<string> SendJsonAsync<T>(
        HttpMethod method,
        string url,
        T body,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.options.AccessToken);
        request.Content = JsonContent.Create(body, options: JsonOptions);

        using HttpResponseMessage response = await this.http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Google Health API returned {(int)response.StatusCode} {response.ReasonPhrase}: {responseBody}");
        }

        return responseBody;
    }
}