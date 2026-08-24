// <copyright file="DiaryImage.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Foody.Gemini;

/// <summary>
/// An image referenced from a food diary markdown file.
/// </summary>
/// <param name="Path">Absolute path to the image file.</param>
/// <param name="Reference">Original reference text from the markdown.</param>
internal sealed record DiaryImage(string Path, string Reference);