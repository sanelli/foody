// <copyright file="AppConfiguration.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Foody.Lib;

using Microsoft.Extensions.Configuration;

namespace Foody;

/// <summary>
/// Loads Foody host configuration from appsettings files.
/// </summary>
internal static class AppConfiguration
{
    private static readonly Lazy<GoogleHealthOptions> GoogleHealthOptionsLazy = new(LoadGoogleHealthOptions);

    /// <summary>
    /// Gets Google Health options from <c>appsettings.json</c> and optional <c>appsettings.Local.json</c>.
    /// </summary>
    public static GoogleHealthOptions GoogleHealth => GoogleHealthOptionsLazy.Value;

    private static GoogleHealthOptions LoadGoogleHealthOptions()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
            .Build();

        return configuration.GetSection("GoogleHealth").Get<GoogleHealthOptions>()
               ?? new GoogleHealthOptions();
    }
}