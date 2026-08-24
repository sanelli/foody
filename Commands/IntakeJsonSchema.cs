// <copyright file="IntakeJsonSchema.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.Json;
using System.Text.Json.Nodes;

using Foody.Models;

namespace Foody.Commands;

/// <summary>
/// JSON Schema for the intake document accepted by <c>foody intake</c>.
/// </summary>
internal static class IntakeJsonSchema
{
    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
    };

    /// <summary>
    /// Builds a pretty-printed JSON Schema document for intake payloads.
    /// </summary>
    /// <returns>JSON Schema text.</returns>
    public static string Create()
    {
        JsonArray mealTypeEnum = new();
        foreach (string mealType in MealTypes.Values)
        {
            mealTypeEnum.Add(mealType);
        }

        JsonObject schema = new()
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

        return schema.ToJsonString(WriteOptions);
    }
}