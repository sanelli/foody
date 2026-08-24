// <copyright file="AppConfiguration.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Foody.Lib;

using Microsoft.Extensions.Configuration;

namespace Foody.Gemini;

/// <summary>
/// Loads Foody.Gemini host configuration from appsettings files.
/// </summary>
internal static class AppConfiguration
{
    private static readonly Lazy<IConfigurationRoot> ConfigurationLazy = new(BuildConfiguration);
    private static readonly Lazy<GoogleHealthOptions> GoogleHealthOptionsLazy = new(LoadGoogleHealthOptions);
    private static readonly Lazy<GeminiOptions> GeminiOptionsLazy = new(LoadGeminiOptions);

    /// <summary>
    /// Gets Google Health options from configuration.
    /// </summary>
    public static GoogleHealthOptions GoogleHealth => GoogleHealthOptionsLazy.Value;

    /// <summary>
    /// Gets Gemini options from configuration.
    /// </summary>
    public static GeminiOptions Gemini => GeminiOptionsLazy.Value;

    private static IConfigurationRoot BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
            .Build();
    }

    private static GoogleHealthOptions LoadGoogleHealthOptions()
    {
        return ConfigurationLazy.Value.GetSection("GoogleHealth").Get<GoogleHealthOptions>()
               ?? new GoogleHealthOptions();
    }

    private static GeminiOptions LoadGeminiOptions()
    {
        return ConfigurationLazy.Value.GetSection("Gemini").Get<GeminiOptions>()
               ?? new GeminiOptions();
    }
}