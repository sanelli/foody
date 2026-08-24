// <copyright file="GoogleHealthOptions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Foody.Lib;

/// <summary>
/// Credentials and settings for Google Health API.
/// Bound from the <c>GoogleHealth</c> section in appsettings.
/// </summary>
public sealed class GoogleHealthOptions
{
    /// <summary>
    /// Required OAuth scope for creating nutrition logs:
    /// https://www.googleapis.com/auth/googlehealth.nutrition.writeonly.
    /// </summary>
    public const string NutritionWriteOnlyScope =
        "https://www.googleapis.com/auth/googlehealth.nutrition.writeonly";

    /// <summary>
    /// Gets or sets the OAuth 2.0 Client ID from Google Cloud Console.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the OAuth 2.0 Client Secret from Google Cloud Console.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the short-lived Bearer access token for API calls.
    /// Obtained via user OAuth consent (authorization code flow).
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the long-lived refresh token (requires access_type=offline during consent).
    /// Used to mint new access tokens without re-prompting the user.
    /// </summary>
    public string? RefreshToken { get; set; }
}