// <copyright file="GeminiOptions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Foody.Gemini;

/// <summary>
/// Gemini Developer API (AI Studio) settings.
/// </summary>
internal sealed class GeminiOptions
{
    /// <summary>
    /// Gets or sets the API key from Google AI Studio.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the Gemini model id.
    /// </summary>
    public string Model { get; set; } = "gemini-2.5-flash";
}