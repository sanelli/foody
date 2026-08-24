// <copyright file="Program.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Foody.Lib;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Foody.Mcp;

/// <summary>
/// Entry point for the Foody MCP server.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Application entry point.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>A task that completes when the host stops.</returns>
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = args,
            ContentRootPath = AppContext.BaseDirectory,
        });

        builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

        builder.Logging.AddConsole(options =>
        {
            // Keep stdout free for the MCP JSON-RPC stream.
            options.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        builder.Services
            .Configure<GoogleHealthOptions>(builder.Configuration.GetSection("GoogleHealth"))
            .AddSingleton(static services =>
            {
                GoogleHealthOptions options = services.GetRequiredService<IOptions<GoogleHealthOptions>>().Value;
                return new FoodyService(options);
            })
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();

        await builder.Build().RunAsync().ConfigureAwait(false);
    }
}