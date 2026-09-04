using System.Globalization;

namespace Void.Client;

internal sealed partial class GameRuntime
{
    sealed class ScreenImage : IDisposable
    {
        private readonly byte[] _imageBytes;
        private readonly int _pixelDataOffset;

        private ScreenImage(byte[] imageBytes, int pixelDataOffset, int width, int height)
        {
            _imageBytes = imageBytes;
            _pixelDataOffset = pixelDataOffset;
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }
        public byte[] Bytes => _imageBytes;

        public static ScreenImage LoadPortablePixmap(byte[] imageBytes)
        {
            var headerOffset = 0;
            var magic = ReadPortablePixmapToken(imageBytes, ref headerOffset);

            if (magic != "P6")
                throw new InvalidOperationException($"Unsupported screen capture format '{magic}'");

            if (!int.TryParse(ReadPortablePixmapToken(imageBytes, ref headerOffset), NumberStyles.None, CultureInfo.InvariantCulture, out var width) || width <= 0)
                throw new InvalidOperationException("Screen capture has an invalid width");

            if (!int.TryParse(ReadPortablePixmapToken(imageBytes, ref headerOffset), NumberStyles.None, CultureInfo.InvariantCulture, out var height) || height <= 0)
                throw new InvalidOperationException("Screen capture has an invalid height");

            if (ReadPortablePixmapToken(imageBytes, ref headerOffset) != "255")
                throw new InvalidOperationException("Screen capture does not use 8-bit color channels");

            var pixelByteCount = checked(width * height * 3);
            var pixelDataOffset = imageBytes.Length - pixelByteCount;

            if (pixelDataOffset <= headerOffset)
                throw new InvalidOperationException("Screen capture pixel data is truncated");

            return new ScreenImage(imageBytes, pixelDataOffset, width, height);
        }

        public bool IsChatInputVisible()
        {
            if (Height is not 2)
                throw new InvalidOperationException($"Chat input analysis requires exactly two pixel rows, but received {Height}");

            var rowByteCount = Width * 3;
            return ChatInputVisibilityDetector.IsVisible(
                _imageBytes.AsSpan(_pixelDataOffset, rowByteCount),
                _imageBytes.AsSpan(_pixelDataOffset + rowByteCount, rowByteCount));
        }

        public void Dispose()
        {
            // The image is stored in managed memory and needs no explicit cleanup.
        }

        private static string ReadPortablePixmapToken(byte[] imageBytes, ref int offset)
        {
            while (offset < imageBytes.Length)
            {
                while (offset < imageBytes.Length && char.IsWhiteSpace((char)imageBytes[offset]))
                    offset++;

                if (offset >= imageBytes.Length || imageBytes[offset] is not (byte)'#')
                    break;

                while (offset < imageBytes.Length && imageBytes[offset] is not (byte)'\n')
                    offset++;
            }

            var tokenStart = offset;

            while (offset < imageBytes.Length && !char.IsWhiteSpace((char)imageBytes[offset]))
                offset++;

            if (tokenStart == offset)
                throw new InvalidOperationException("Screen capture has an incomplete portable pixmap header");

            return System.Text.Encoding.ASCII.GetString(imageBytes, tokenStart, offset - tokenStart);
        }
    }

    sealed record ConnectionScreenObservation(ConnectionNavigationKind Kind, ConnectionTextMatch TextMatch);

    sealed record ConnectionScreenRecognition(IReadOnlyDictionary<ConnectionTextAction, ConnectionTextMatch> Matches, ConnectionScreenObservation? Observation);
}

internal static class ChatInputVisibilityDetector
{
    private const double MinimumBrightness = 1;
    private const double BrightnessRatioThreshold = 0.7;

    public static bool IsVisible(ReadOnlySpan<byte> pixelsAboveInput, ReadOnlySpan<byte> inputPixels)
    {
        if (pixelsAboveInput.Length != inputPixels.Length)
            throw new ArgumentException("Chat input rows must have matching lengths", nameof(inputPixels));

        var brightnessAboveInput = CalculateAverageLuminance(pixelsAboveInput);
        var inputBrightness = CalculateAverageLuminance(inputPixels);
        return brightnessAboveInput >= MinimumBrightness
            && inputBrightness / brightnessAboveInput <= BrightnessRatioThreshold;
    }

    internal static double CalculateAverageLuminance(ReadOnlySpan<byte> pixels)
    {
        if (pixels.IsEmpty || pixels.Length % 3 is not 0)
            throw new ArgumentException("Pixel data must contain complete RGB pixels", nameof(pixels));

        double totalLuminance = 0;

        for (var offset = 0; offset < pixels.Length; offset += 3)
            totalLuminance += pixels[offset] * 0.2126 + pixels[offset + 1] * 0.7152 + pixels[offset + 2] * 0.0722;

        return totalLuminance / (pixels.Length / 3) / byte.MaxValue * 100;
    }
}
