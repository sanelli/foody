// <copyright file="MealTypes.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Foody.Lib;

/// <summary>
/// Meal type values accepted by Google Health API <c>nutrition-log</c>.
/// </summary>
public static class MealTypes
{
    /// <summary>
    /// Gets the meal type values that may be set on an intake payload.
    /// </summary>
    public static IReadOnlyList<string> Values { get; } =
    [
        "BREAKFAST",
        "LUNCH",
        "DINNER",
        "SNACK",
    ];
}