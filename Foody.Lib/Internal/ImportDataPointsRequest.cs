// <copyright file="ImportDataPointsRequest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Foody.Lib.Internal;

/// <summary>
/// Request body for
/// POST https://health.googleapis.com/v4/users/me/dataTypes/nutrition-log/dataPoints:import.
/// </summary>
internal sealed class ImportDataPointsRequest
{
    /// <summary>
    /// Gets the inline source containing data points to import.
    /// </summary>
    [JsonPropertyName("inlineSource")]
    public required ImportInlineSource InlineSource { get; init; }
}