// <copyright file="NutritionLog.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Foody.Models;

/// <summary>
/// Nutrition log fields accepted by Google Health API.
/// </summary>
internal sealed class NutritionLog
{
    /// <summary>
    /// Gets the time interval for the intake.
    /// </summary>
    [JsonPropertyName("interval")]
    public required TimeInterval Interval { get; init; }

    /// <summary>
    /// Gets the display name of the food (anonymous food path).
    /// </summary>
    [JsonPropertyName("foodDisplayName")]
    public required string FoodDisplayName { get; init; }

    /// <summary>
    /// Gets the meal type.
    /// </summary>
    [JsonPropertyName("mealType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MealType { get; init; }

    /// <summary>
    /// Gets energy quantity.
    /// </summary>
    [JsonPropertyName("energy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EnergyQuantity? Energy { get; init; }

    /// <summary>
    /// Gets total carbohydrate quantity.
    /// </summary>
    [JsonPropertyName("totalCarbohydrate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MassQuantity? TotalCarbohydrate { get; init; }

    /// <summary>
    /// Gets total fat quantity.
    /// </summary>
    [JsonPropertyName("totalFat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MassQuantity? TotalFat { get; init; }

    /// <summary>
    /// Gets protein quantity.
    /// </summary>
    [JsonPropertyName("protein")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MassQuantity? Protein { get; init; }
}