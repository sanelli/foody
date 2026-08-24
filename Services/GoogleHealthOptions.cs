// <copyright file="GoogleHealthOptions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Foody.Services;

/// <summary>
/// Credentials and settings for Google Health API.
/// None of these are wired yet — populate via env vars or a secrets file later.
/// </summary>
internal sealed class GoogleHealthOptions
{
    /// <summary>
    /// Required OAuth scope for creating nutrition logs:
    /// https://www.googleapis.com/auth/googlehealth.nutrition.writeonly.
    /// </summary>
    public const string NutritionWriteOnlyScope =
        "https://www.googleapis.com/auth/googlehealth.nutrition.writeonly";

    /// <summary>
    /// Gets the OAuth 2.0 Client ID from Google Cloud Console.
    /// </summary>
    public string? ClientId { get; init; }

    /// <summary>
    /// Gets the OAuth 2.0 Client Secret from Google Cloud Console.
    /// </summary>
    public string? ClientSecret { get; init; }

    /// <summary>
    /// Gets the short-lived Bearer access token for API calls.
    /// Obtained via user OAuth consent (authorization code flow).
    /// </summary>
    public string? AccessToken { get; init; }

    /// <summary>
    /// Gets the long-lived refresh token (requires access_type=offline during consent).
    /// Used to mint new access tokens without re-prompting the user.
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Builds options from <c>FOODY_GOOGLE_*</c> environment variables.
    /// </summary>
    /// <returns>A new <see cref="GoogleHealthOptions"/> instance.</returns>
    public static GoogleHealthOptions FromEnvironment()
    {
        return new GoogleHealthOptions
        {
            ClientId = Environment.GetEnvironmentVariable("FOODY_GOOGLE_CLIENT_ID"),
            ClientSecret = Environment.GetEnvironmentVariable("FOODY_GOOGLE_CLIENT_SECRET"),
            AccessToken = Environment.GetEnvironmentVariable("FOODY_GOOGLE_ACCESS_TOKEN"),
            RefreshToken = Environment.GetEnvironmentVariable("FOODY_GOOGLE_REFRESH_TOKEN"),
        };
    }
}