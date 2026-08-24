// <copyright file="FoodIntake.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Foody.Models;

/// <summary>
/// Local intake payload read from a JSON file passed to <c>foody intake</c>.
/// </summary>
[SuppressMessage(
    "Microsoft.Performance",
    "CA1812:AvoidUninstantiatedInternalClasses",
    Justification = "Instantiated by System.Text.Json deserialization.")]
internal sealed class FoodIntake
{
    /// <summary>
    /// Gets the display name of the food item.
    /// </summary>
    [JsonPropertyName("foodDisplayName")]
    public required string FoodDisplayName { get; init; }

    /// <summary>
    /// Gets the meal type (for example <c>BREAKFAST</c>, <c>LUNCH</c>, <c>SNACK</c>).
    /// </summary>
    [JsonPropertyName("mealType")]
    public string? MealType { get; init; }

    /// <summary>
    /// Gets the intake interval start time.
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTimeOffset? StartTime { get; init; }

    /// <summary>
    /// Gets the intake interval end time.
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTimeOffset? EndTime { get; init; }

    /// <summary>
    /// Gets energy in kilocalories.
    /// </summary>
    [JsonPropertyName("energyKcal")]
    public double? EnergyKcal { get; init; }

    /// <summary>
    /// Gets total carbohydrate in grams.
    /// </summary>
    [JsonPropertyName("totalCarbohydrateGrams")]
    public double? TotalCarbohydrateGrams { get; init; }

    /// <summary>
    /// Gets total fat in grams.
    /// </summary>
    [JsonPropertyName("totalFatGrams")]
    public double? TotalFatGrams { get; init; }

    /// <summary>
    /// Gets total protein in grams.
    /// </summary>
    [JsonPropertyName("totalProteinGrams")]
    public double? TotalProteinGrams { get; init; }
}