// <copyright file="SampleIntakeJson.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Foody.Commands;

/// <summary>
/// Canonical sample intake JSON used by <c>foody sample</c>.
/// </summary>
internal static class SampleIntakeJson
{
    /// <summary>
    /// Gets the sample intake JSON document (pretty-printed).
    /// </summary>
    public const string Content =
        """
        {
          "foodDisplayName": "Banana",
          "mealType": "SNACK",
          "startTime": "2026-08-24T15:00:00Z",
          "endTime": "2026-08-24T15:05:00Z",
          "energyKcal": 105,
          "totalCarbohydrateGrams": 27,
          "totalFatGrams": 0.3,
          "totalProteinGrams": 1.3
        }
        """;
}