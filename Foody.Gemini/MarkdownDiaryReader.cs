// <copyright file="MarkdownDiaryReader.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

namespace Foody.Gemini;

/// <summary>
/// Reads a markdown food diary and resolves local image references.
/// </summary>
internal static partial class MarkdownDiaryReader
{
    /// <summary>
    /// Reads markdown text and collects image references that resolve under the markdown folder.
    /// </summary>
    /// <param name="markdownPath">Path to the markdown file.</param>
    /// <returns>Markdown content and resolved local images.</returns>
    public static async Task<(string Markdown, IReadOnlyList<DiaryImage> Images)> ReadAsync(string markdownPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(markdownPath);

        string fullPath = Path.GetFullPath(markdownPath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Markdown file not found: {fullPath}", fullPath);
        }

        string folder = Path.GetDirectoryName(fullPath)
                        ?? throw new InvalidOperationException($"Unable to resolve directory for '{fullPath}'.");
        string markdown = await File.ReadAllTextAsync(fullPath).ConfigureAwait(false);

        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);
        List<DiaryImage> images = [];

        foreach (Match match in MarkdownImageRegex().Matches(markdown))
        {
            string reference = match.Groups["path"].Value.Trim();
            TryAddImage(folder, reference, seen, images);
        }

        foreach (Match match in HtmlImageRegex().Matches(markdown))
        {
            string reference = match.Groups["path"].Value.Trim();
            TryAddImage(folder, reference, seen, images);
        }

        return (markdown, images);
    }

    private static void TryAddImage(
        string folder,
        string reference,
        HashSet<string> seen,
        List<DiaryImage> images)
    {
        if (string.IsNullOrWhiteSpace(reference)
            || reference.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || reference.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || reference.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        string candidate = Path.IsPathRooted(reference)
            ? Path.GetFullPath(reference)
            : Path.GetFullPath(Path.Combine(folder, reference));

        string folderFull = Path.GetFullPath(folder);
        if (!candidate.StartsWith(folderFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(candidate, folderFull, StringComparison.OrdinalIgnoreCase)
            && !candidate.StartsWith(folderFull + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            // Only accept images under the markdown directory.
            return;
        }

        if (!File.Exists(candidate) || !seen.Add(candidate))
        {
            return;
        }

        images.Add(new DiaryImage(candidate, reference));
    }

    [GeneratedRegex(@"!\[[^\]]*\]\((?<path>[^)\s]+)(?:\s+""[^""]*"")?\)", RegexOptions.CultureInvariant)]
    private static partial Regex MarkdownImageRegex();

    [GeneratedRegex(@"<img\b[^>]*\bsrc\s*=\s*[""'](?<path>[^""']+)[""'][^>]*>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex HtmlImageRegex();
}