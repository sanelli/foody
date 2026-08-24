// <copyright file="JsonResponseExtractor.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text.Json;

namespace Foody.Gemini;

/// <summary>
/// Extracts a JSON array payload from a model response that may include surrounding text.
/// </summary>
internal static class JsonResponseExtractor
{
    /// <summary>
    /// Extracts the first JSON array from <paramref name="text"/>.
    /// </summary>
    /// <param name="text">Raw model response.</param>
    /// <returns>JSON array text.</returns>
    public static string ExtractJsonArray(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        string trimmed = text.Trim();
        if (trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            trimmed = StripMarkdownFence(trimmed);
        }

        if (TryParseArray(trimmed, out string? direct))
        {
            return direct!;
        }

        int start = trimmed.IndexOf('[', StringComparison.Ordinal);
        while (start >= 0)
        {
            if (TryExtractBalancedArray(trimmed, start, out string? candidate)
                && TryParseArray(candidate!, out string? parsed))
            {
                return parsed!;
            }

            start = trimmed.IndexOf('[', start + 1);
        }

        throw new InvalidOperationException("Gemini response did not contain a valid JSON array.");
    }

    private static string StripMarkdownFence(string text)
    {
        string withoutOpening = text;
        if (withoutOpening.StartsWith("```", StringComparison.Ordinal))
        {
            int firstNewline = withoutOpening.IndexOf('\n', StringComparison.Ordinal);
            withoutOpening = firstNewline >= 0 ? withoutOpening[(firstNewline + 1)..] : withoutOpening[3..];
        }

        int fence = withoutOpening.LastIndexOf("```", StringComparison.Ordinal);
        if (fence >= 0)
        {
            withoutOpening = withoutOpening[..fence];
        }

        return withoutOpening.Trim();
    }

    private static bool TryExtractBalancedArray(string text, int start, out string? json)
    {
        json = null;
        int depth = 0;
        bool inString = false;
        bool escape = false;

        for (int i = start; i < text.Length; i++)
        {
            char c = text[i];
            if (inString)
            {
                if (escape)
                {
                    escape = false;
                }
                else if (c == '\\')
                {
                    escape = true;
                }
                else if (c == '"')
                {
                    inString = false;
                }

                continue;
            }

            switch (c)
            {
                case '"':
                    inString = true;
                    break;
                case '[':
                    depth++;
                    break;
                case ']':
                    depth--;
                    if (depth == 0)
                    {
                        json = text[start..(i + 1)];
                        return true;
                    }

                    break;
            }
        }

        return false;
    }

    private static bool TryParseArray(string candidate, out string? normalized)
    {
        normalized = null;
        try
        {
            using JsonDocument document = JsonDocument.Parse(candidate);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                return false;
            }

            normalized = document.RootElement.GetRawText();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}