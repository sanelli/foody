// <copyright file="BulkIntakeLimits.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Foody.Lib;

/// <summary>
/// Limits for <c>bulk-intake</c> payloads.
/// </summary>
public static class BulkIntakeLimits
{
    /// <summary>
    /// Maximum number of intake items accepted in one bulk-intake request.
    /// Matches the Google Health API import limit.
    /// </summary>
    public const int MaxCount = 10000;
}