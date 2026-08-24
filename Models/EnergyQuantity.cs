// <copyright file="EnergyQuantity.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Foody.Models;

/// <summary>
/// Energy quantity in kilocalories.
/// </summary>
internal sealed class EnergyQuantity
{
    /// <summary>
    /// Gets kilocalories.
    /// </summary>
    [JsonPropertyName("kcal")]
    public required double Kcal { get; init; }
}