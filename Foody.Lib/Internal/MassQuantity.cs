// <copyright file="MassQuantity.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Foody.Lib.Internal;

/// <summary>
/// Mass quantity in grams.
/// </summary>
internal sealed class MassQuantity
{
    /// <summary>
    /// Gets grams.
    /// </summary>
    [JsonPropertyName("grams")]
    public required double Grams { get; init; }
}