// <copyright file="ImportInlineSource.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Foody.Lib.Internal;

/// <summary>
/// Inline list of data points for an import request.
/// </summary>
internal sealed class ImportInlineSource
{
    /// <summary>
    /// Gets the data points to import.
    /// </summary>
    [JsonPropertyName("dataPoints")]
    public required IReadOnlyList<NutritionLogDataPointRequest> DataPoints { get; init; }
}