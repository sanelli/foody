// <copyright file="TimeInterval.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Foody.Models;

/// <summary>
/// Time interval for a nutrition log entry.
/// </summary>
internal sealed class TimeInterval
{
    /// <summary>
    /// Gets the interval start time in RFC 3339 UTC form.
    /// </summary>
    [JsonPropertyName("startTime")]
    public required string StartTime { get; init; }

    /// <summary>
    /// Gets the interval end time in RFC 3339 UTC form.
    /// </summary>
    [JsonPropertyName("endTime")]
    public required string EndTime { get; init; }
}