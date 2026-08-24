// <copyright file="IntakeJsonSchema.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Foody.Lib;

/// <summary>
/// JSON Schema documents accepted by Foody commands.
/// </summary>
internal static class IntakeJsonSchema
{
    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
    };

    /// <summary>
    /// Builds a pretty-printed JSON Schema for the given schema kind.
    /// </summary>
    /// <param name="kind">Schema kind (<see cref="SchemaKind.Intake"/> or <see cref="SchemaKind.BulkIntake"/>).</param>
    /// <returns>JSON Schema text.</returns>
    public static string Create(string kind)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(kind);

        string normalized = kind.Trim();
        if (string.Equals(normalized, SchemaKind.Intake, StringComparison.OrdinalIgnoreCase))
        {
            return CreateIntakeObjectSchema().ToJsonString(WriteOptions);
        }

        if (string.Equals(normalized, SchemaKind.BulkIntake, StringComparison.OrdinalIgnoreCase))
        {
            return CreateBulkIntakeSchema().ToJsonString(WriteOptions);
        }

        throw new ArgumentException(
            $"Unknown schema kind '{kind}'. Supported values: {string.Join(", ", SchemaKind.Values)}.",
            nameof(kind));
    }

    private static JsonObject CreateIntakeObjectSchema()
    {
        JsonArray mealTypeEnum = new();
        foreach (string mealType in MealTypes.Values)
        {
            mealTypeEnum.Add(mealType);
        }

        return new JsonObject
        {
            ["$schema"] = "https://json-schema.org/draft/2020-12/schema",
            ["$id"] = "https://foody.local/schemas/intake.json",
            ["title"] = "FoodyIntake",
            ["description"] = "Food intake payload accepted by foody intake.",
            ["type"] = "object",
            ["additionalProperties"] = false,
            ["required"] = new JsonArray("foodDisplayName"),
            ["properties"] = new JsonObject
            {
                ["foodDisplayName"] = new JsonObject
                {
                    ["type"] = "string",
                    ["minLength"] = 1,
                    ["description"] = "Display name of the food item.",
                },
                ["mealType"] = new JsonObject
                {
                    ["type"] = "string",
                    ["enum"] = mealTypeEnum,
                    ["description"] = "Meal category for the intake.",
                },
                ["startTime"] = new JsonObject
                {
                    ["type"] = "string",
                    ["format"] = "date-time",
                    ["description"] = "Intake interval start time (RFC 3339).",
                },
                ["endTime"] = new JsonObject
                {
                    ["type"] = "string",
                    ["format"] = "date-time",
                    ["description"] = "Intake interval end time (RFC 3339).",
                },
                ["energyKcal"] = new JsonObject
                {
                    ["type"] = "number",
                    ["minimum"] = 0,
                    ["description"] = "Energy in kilocalories.",
                },
                ["totalCarbohydrateGrams"] = new JsonObject
                {
                    ["type"] = "number",
                    ["minimum"] = 0,
                    ["description"] = "Total carbohydrate in grams.",
                },
                ["totalFatGrams"] = new JsonObject
                {
                    ["type"] = "number",
                    ["minimum"] = 0,
                    ["description"] = "Total fat in grams.",
                },
                ["totalProteinGrams"] = new JsonObject
                {
                    ["type"] = "number",
                    ["minimum"] = 0,
                    ["description"] = "Total protein in grams.",
                },
            },
        };
    }

    private static JsonObject CreateBulkIntakeSchema()
    {
        JsonObject itemSchema = CreateIntakeObjectSchema();
        _ = itemSchema.Remove("$schema");
        _ = itemSchema.Remove("$id");
        itemSchema["title"] = "FoodyIntakeItem";
        itemSchema["description"] = "One food intake payload inside a bulk-intake array.";

        return new JsonObject
        {
            ["$schema"] = "https://json-schema.org/draft/2020-12/schema",
            ["$id"] = "https://foody.local/schemas/bulk-intake.json",
            ["title"] = "FoodyBulkIntake",
            ["description"] =
                $"JSON array of intake payloads accepted by foody bulk-intake (1 to {BulkIntakeLimits.MaxCount.ToString(CultureInfo.InvariantCulture)} items).",
            ["type"] = "array",
            ["minItems"] = 1,
            ["maxItems"] = BulkIntakeLimits.MaxCount,
            ["items"] = itemSchema,
        };
    }
}