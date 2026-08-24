// <copyright file="FoodyTools.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.ComponentModel;

using Foody.Lib;

using ModelContextProtocol.Server;

namespace Foody.Mcp;

/// <summary>
/// MCP tools mirroring the Foody CLI features.
/// </summary>
[McpServerToolType]
internal static class FoodyTools
{
    /// <summary>
    /// Parses intake JSON and logs it to Google Health API.
    /// </summary>
    /// <param name="service">Foody service.</param>
    /// <param name="json">Intake JSON document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Google Health API response JSON.</returns>
    [McpServerTool(Name = "intake")]
    [Description("Parse intake JSON and log it to Google Health API.")]
    public static Task<string> IntakeAsync(
        FoodyService service,
        [Description("Intake JSON document matching the Foody schema.")] string json,
        CancellationToken cancellationToken)
    {
        return service.LogIntakeAsync(json, cancellationToken);
    }

    /// <summary>
    /// Returns a sample intake JSON document.
    /// </summary>
    /// <returns>Sample JSON text.</returns>
    [McpServerTool(Name = "sample")]
    [Description("Return a sample intake JSON document.")]
    public static string Sample()
    {
        return FoodyService.GetSampleJson();
    }

    /// <summary>
    /// Returns the JSON Schema for intake documents.
    /// </summary>
    /// <returns>JSON Schema text.</returns>
    [McpServerTool(Name = "schema")]
    [Description("Return the JSON Schema for intake documents accepted by intake.")]
    public static string Schema()
    {
        return FoodyService.GetSchemaJson();
    }

    /// <summary>
    /// Lists possible meal type values.
    /// </summary>
    /// <returns>Meal type values, one per line.</returns>
    [McpServerTool(Name = "list_mealtype")]
    [Description("List possible meal type values for intake JSON.")]
    public static string ListMealType()
    {
        return FoodyService.ListMealTypes();
    }

    /// <summary>
    /// Returns Foody help text.
    /// </summary>
    /// <returns>Help text.</returns>
    [McpServerTool(Name = "help")]
    [Description("Show help and usage information.")]
    public static string Help()
    {
        return FoodyService.GetHelpText();
    }
}