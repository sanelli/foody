// <copyright file="SchemaKind.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Foody.Lib;

/// <summary>
/// Identifiers for JSON schemas exposed by Foody.
/// </summary>
public static class SchemaKind
{
    /// <summary>
    /// Single intake object schema used by <c>intake</c>.
    /// </summary>
    public const string Intake = "intake";

    /// <summary>
    /// JSON array of intake objects used by <c>bulk-intake</c>.
    /// </summary>
    public const string BulkIntake = "bulk-intake";

    /// <summary>
    /// Gets all supported schema kind identifiers.
    /// </summary>
    public static IReadOnlyList<string> Values { get; } =
    [
        Intake,
        BulkIntake,
    ];
}