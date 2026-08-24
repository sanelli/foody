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
        [Description("Intake JSON document matching the Foody intake schema.")] string json,
        CancellationToken cancellationToken)
    {
        return service.LogIntakeAsync(json, cancellationToken);
    }

    /// <summary>
    /// Imports a JSON array of intake objects via Google Health API.
    /// </summary>
    /// <param name="service">Foody service.</param>
    /// <param name="json">JSON array of intake documents (max 10000).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Google Health API response JSON.</returns>
    [McpServerTool(Name = "bulk_intake")]
    [Description("Import multiple intake JSON objects via Google Health API (JSON array, max 10000 items).")]
    public static Task<string> BulkIntakeAsync(
        FoodyService service,
        [Description("JSON array of intake documents matching the Foody bulk-intake schema.")] string json,
        CancellationToken cancellationToken)
    {
        return service.LogBulkIntakeAsync(json, cancellationToken);
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
    /// Returns the JSON Schema for the selected kind.
    /// </summary>
    /// <param name="kind">Schema kind: intake or bulk-intake.</param>
    /// <returns>JSON Schema text.</returns>
    [McpServerTool(Name = "schema")]
    [Description("Return the JSON Schema for intake or bulk-intake payloads.")]
    public static string Schema(
        [Description("Schema kind: 'intake' or 'bulk-intake'.")] string kind)
    {
        return FoodyService.GetSchemaJson(kind);
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
    /// Lists available schema kinds.
    /// </summary>
    /// <returns>Schema kinds, one per line.</returns>
    [McpServerTool(Name = "list_schema")]
    [Description("List available schema kinds (intake, bulk-intake).")]
    public static string ListSchema()
    {
        return FoodyService.ListSchemaKinds();
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