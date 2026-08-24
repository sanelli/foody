// <copyright file="NutritionLogDataPointRequest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Foody.Models;

/// <summary>
/// Request body for
/// POST https://health.googleapis.com/v4/users/me/dataTypes/nutrition-log/dataPoints.
/// </summary>
internal sealed class NutritionLogDataPointRequest
{
    /// <summary>
    /// Gets the nutrition log payload.
    /// </summary>
    [JsonPropertyName("nutritionLog")]
    public required NutritionLog NutritionLog { get; init; }
}