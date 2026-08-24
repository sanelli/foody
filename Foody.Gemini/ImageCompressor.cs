// <copyright file="ImageCompressor.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Foody.Gemini;

/// <summary>
/// Compresses images so they stay under a maximum byte size before upload.
/// </summary>
internal static class ImageCompressor
{
    /// <summary>
    /// Maximum encoded image size in bytes.
    /// </summary>
    public const int MaxBytes = 250 * 1024;

    /// <summary>
    /// Loads an image and returns JPEG bytes no larger than <see cref="MaxBytes"/>.
    /// </summary>
    /// <param name="path">Image path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>JPEG bytes and MIME type.</returns>
    public static async Task<(byte[] Bytes, string MimeType)> CompressAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        FileStream input = File.OpenRead(path);
        await using (input.ConfigureAwait(false))
        {
            using Image image = await Image.LoadAsync(input, cancellationToken).ConfigureAwait(false);

            int quality = 85;
            double scale = 1.0;

            for (int attempt = 0; attempt < 24; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using Image frame = image.Clone(ctx =>
                {
                    if (scale < 0.999)
                    {
                        int width = Math.Max(1, (int)Math.Round(image.Width * scale));
                        int height = Math.Max(1, (int)Math.Round(image.Height * scale));
                        ctx.Resize(width, height);
                    }
                });

                MemoryStream output = new();
                await using (output.ConfigureAwait(false))
                {
                    JpegEncoder encoder = new() { Quality = quality };
                    await frame.SaveAsJpegAsync(output, encoder, cancellationToken).ConfigureAwait(false);
                    byte[] bytes = output.ToArray();
                    if (bytes.Length <= MaxBytes)
                    {
                        return (bytes, "image/jpeg");
                    }
                }

                if (quality > 40)
                {
                    quality -= 10;
                }
                else
                {
                    quality = 75;
                    scale *= 0.8;
                }
            }
        }

        throw new InvalidOperationException(
            $"Unable to compress '{path}' below {MaxBytes} bytes.");
    }
}