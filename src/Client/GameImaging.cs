using System.Globalization;

namespace Void.Client;

internal sealed partial class GameRuntime
{
    sealed class ScreenImage : IDisposable
    {
        private const int MinimumButtonWidth = 60;
        private const int MaximumButtonWidth = 430;
        private const int MinimumButtonHeight = 36;
        private const int MaximumButtonHeight = 42;
        private const byte BlackThreshold = 12;
        private const byte NeutralColorTolerance = 16;

        private readonly byte[] _imageBytes;
        private readonly int _pixelDataOffset;
        private IReadOnlyList<ScreenRectangle>? _buttons;

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

        public bool TryFindPauseMenuBackToGameButton(out ScreenRectangle backToGameButton)
        {
            var buttons = FindButtons();
            var centeredWideButtons = buttons
                .Where(button => button.Width >= Width * 0.4 && IsHorizontallyCentered(button))
                .OrderBy(button => button.Top)
                .ToArray();

            if (buttons.Count >= 5
                && centeredWideButtons.Length >= 2
                && centeredWideButtons[0].Top >= Height * 0.2
                && centeredWideButtons[0].Top <= Height * 0.45
                && centeredWideButtons[^1].Top >= Height * 0.6)
            {
                backToGameButton = centeredWideButtons[0];
                return true;
            }

            backToGameButton = default;
            return false;
        }

        public ScreenRectangle FindInteractionArea(OcrRectangle textBounds)
        {
            var containingButton = FindButtons()
                .Where(button => textBounds.CenterX >= button.Left && textBounds.CenterX <= button.Right)
                .Where(button => textBounds.CenterY >= button.Top && textBounds.CenterY <= button.Bottom)
                .OrderBy(button => button.Width * button.Height)
                .FirstOrDefault();

            if (containingButton != default)
                return containingButton;

            const int horizontalPadding = 24;
            const int verticalPadding = 10;
            var left = Math.Max(0, textBounds.Left - horizontalPadding);
            var top = Math.Max(0, textBounds.Top - verticalPadding);
            var right = Math.Min(Width, textBounds.Right + horizontalPadding);
            var bottom = Math.Min(Height, textBounds.Bottom + verticalPadding);
            return new ScreenRectangle(left, top, right - left, bottom - top);
        }

        public bool TryFindServerAddressField(OcrRectangle serverAddressTextBounds, out ScreenRectangle serverAddressField)
        {
            var maximumVerticalDistance = Math.Max(48, serverAddressTextBounds.Height * 6);
            var candidate = FindTextFields()
                .Where(field => serverAddressTextBounds.CenterX >= field.Left && serverAddressTextBounds.CenterX <= field.Right)
                .Where(field => field.Top >= serverAddressTextBounds.Top)
                .Where(field => field.Top - serverAddressTextBounds.Bottom <= maximumVerticalDistance)
                .OrderBy(field => Math.Max(0, field.Top - serverAddressTextBounds.Bottom))
                .ThenBy(field => field.Width * field.Height)
                .FirstOrDefault();

            if (candidate == default)
            {
                serverAddressField = default;
                return false;
            }

            serverAddressField = candidate;
            return true;
        }

        public double CalculateDifferenceRatio(ScreenImage other, ScreenRectangle area, byte channelDifferenceThreshold = 20)
        {
            if (Width != other.Width || Height != other.Height)
                throw new ArgumentException("Screens must have matching dimensions.", nameof(other));

            if (area.Left < 0 || area.Top < 0 || area.Right > Width || area.Bottom > Height)
                throw new ArgumentOutOfRangeException(nameof(area));

            var differentPixels = 0;
            var comparedPixels = area.Width * area.Height;

            for (var y = area.Top; y < area.Bottom; y++)
            {
                for (var x = area.Left; x < area.Right; x++)
                {
                    var leftPixel = GetPixel(x, y);
                    var rightPixel = other.GetPixel(x, y);

                    if (Math.Abs(leftPixel.Red - rightPixel.Red) > channelDifferenceThreshold
                        || Math.Abs(leftPixel.Green - rightPixel.Green) > channelDifferenceThreshold
                        || Math.Abs(leftPixel.Blue - rightPixel.Blue) > channelDifferenceThreshold)
                    {
                        differentPixels++;
                    }
                }
            }

            return comparedPixels is 0 ? 0 : (double)differentPixels / comparedPixels;
        }

        public bool IsServerAddressFieldEmpty(ScreenRectangle serverAddressField)
        {
            const int fieldContentInset = 6;
            const int maximumHorizontalCursorWidth = 12;
            const int maximumHorizontalCursorHeight = 3;
            const int maximumVerticalCursorWidth = 3;

            var fieldContent = serverAddressField.Inset(fieldContentInset);
            var brightPixelCount = 0;
            var brightPixelLeft = fieldContent.Right;
            var brightPixelTop = fieldContent.Bottom;
            var brightPixelRight = fieldContent.Left;
            var brightPixelBottom = fieldContent.Top;

            for (var y = fieldContent.Top; y < fieldContent.Bottom; y++)
            {
                for (var x = fieldContent.Left; x < fieldContent.Right; x++)
                {
                    if (!IsNeutralAndBright(GetPixel(x, y)))
                        continue;

                    brightPixelCount++;
                    brightPixelLeft = Math.Min(brightPixelLeft, x);
                    brightPixelTop = Math.Min(brightPixelTop, y);
                    brightPixelRight = Math.Max(brightPixelRight, x + 1);
                    brightPixelBottom = Math.Max(brightPixelBottom, y + 1);
                }
            }

            if (brightPixelCount is 0)
                return true;

            var brightPixelWidth = brightPixelRight - brightPixelLeft;
            var brightPixelHeight = brightPixelBottom - brightPixelTop;
            var isHorizontalCursor = brightPixelWidth <= maximumHorizontalCursorWidth
                && brightPixelHeight <= maximumHorizontalCursorHeight
                && brightPixelTop >= fieldContent.Top + fieldContent.Height / 2;
            var isVerticalCursor = brightPixelWidth <= maximumVerticalCursorWidth
                && brightPixelHeight >= fieldContent.Height / 3;

            return isHorizontalCursor || isVerticalCursor;
        }

        public void Dispose()
        {
            // The image is stored in managed memory and needs no explicit cleanup.
        }

        public override string ToString()
        {
            return $"{Width}x{Height}; buttons: {string.Join(", ", FindButtons())}";
        }

        private IReadOnlyList<ScreenRectangle> FindButtons()
        {
            return _buttons ??= DetectButtons();
        }

        private IReadOnlyList<ScreenRectangle> DetectButtons()
        {
            var candidates = new List<ScreenRectangle>();

            for (var top = 0; top <= Height - MinimumButtonHeight; top++)
            {
                foreach (var (left, width) in FindBlackRuns(top))
                {
                    if (width is < MinimumButtonWidth or > MaximumButtonWidth)
                        continue;

                    for (var height = MinimumButtonHeight; height <= MaximumButtonHeight && top + height <= Height; height++)
                    {
                        var candidate = new ScreenRectangle(left, top, width, height);

                        if (!HasButtonBorder(candidate) || !HasButtonInterior(candidate))
                            continue;

                        candidates.Add(candidate);
                        break;
                    }
                }
            }

            var buttons = new List<ScreenRectangle>();

            foreach (var candidate in candidates.OrderBy(button => button.Top).ThenBy(button => button.Left))
            {
                var duplicateIndex = buttons.FindIndex(button => AreDuplicateDetections(button, candidate));

                if (duplicateIndex < 0)
                {
                    buttons.Add(candidate);
                    continue;
                }

                if (candidate.Width * candidate.Height > buttons[duplicateIndex].Width * buttons[duplicateIndex].Height)
                    buttons[duplicateIndex] = candidate;
            }

            return buttons.OrderBy(button => button.Top).ThenBy(button => button.Left).ToArray();
        }

        private IEnumerable<(int Left, int Width)> FindBlackRuns(int y)
        {
            var left = 0;

            while (left < Width)
            {
                while (left < Width && !IsBlack(GetPixel(left, y)))
                    left++;

                var right = left;

                while (right < Width && IsBlack(GetPixel(right, y)))
                    right++;

                if (right > left)
                    yield return (left, right - left);

                left = right + 1;
            }
        }

        private bool HasButtonBorder(ScreenRectangle candidate)
        {
            var topBlackPixels = 0;
            var bottomBlackPixels = 0;

            for (var x = candidate.Left; x < candidate.Right; x++)
            {
                if (IsBlack(GetPixel(x, candidate.Top)))
                    topBlackPixels++;

                if (IsBlack(GetPixel(x, candidate.Bottom - 1)))
                    bottomBlackPixels++;
            }

            if ((double)topBlackPixels / candidate.Width < 0.9 || (double)bottomBlackPixels / candidate.Width < 0.85)
                return false;

            var leftBlackPixels = 0;
            var rightBlackPixels = 0;

            for (var y = candidate.Top; y < candidate.Bottom; y++)
            {
                if (IsBlack(GetPixel(candidate.Left, y)))
                    leftBlackPixels++;

                if (IsBlack(GetPixel(candidate.Right - 1, y)))
                    rightBlackPixels++;
            }

            return (double)leftBlackPixels / candidate.Height >= 0.75 && (double)rightBlackPixels / candidate.Height >= 0.75;
        }

        private bool HasButtonInterior(ScreenRectangle candidate)
        {
            var inset = Math.Max(3, candidate.Height / 10);
            var sideSectionWidth = Math.Max(1, candidate.Width / 4 - inset);
            var neutralPixels = 0;
            var sufficientlyBrightPixels = 0;
            var sampleCount = 0;

            for (var y = candidate.Top + inset; y < candidate.Bottom - inset; y += 2)
            {
                foreach (var x in EnumerateSideSectionPixels(candidate, inset, sideSectionWidth))
                {
                    var pixel = GetPixel(x, y);
                    sampleCount++;

                    if (IsNeutral(pixel))
                        neutralPixels++;

                    if (GetBrightness(pixel) >= 24)
                        sufficientlyBrightPixels++;
                }
            }

            return sampleCount > 0
                && (double)neutralPixels / sampleCount >= 0.7
                && (double)sufficientlyBrightPixels / sampleCount >= 0.7;
        }

        private IEnumerable<int> EnumerateSideSectionPixels(ScreenRectangle candidate, int inset, int sideSectionWidth)
        {
            var leftStart = candidate.Left + inset;
            var rightStart = candidate.Right - inset - sideSectionWidth;

            for (var offset = 0; offset < sideSectionWidth; offset += 2)
            {
                yield return leftStart + offset;
                yield return rightStart + offset;
            }
        }

        private IEnumerable<(int Left, int Width)> FindNeutralBrightRuns(int y)
        {
            var left = 0;

            while (left < Width)
            {
                while (left < Width && !IsNeutralAndBright(GetPixel(left, y)))
                    left++;

                var right = left;

                while (right < Width && IsNeutralAndBright(GetPixel(right, y)))
                    right++;

                if (right > left)
                    yield return (left, right - left);

                left = right + 1;
            }
        }

        private IReadOnlyList<ScreenRectangle> FindTextFields()
        {
            const int minimumTextFieldWidth = 80;
            const int minimumTextFieldHeight = 18;
            const int maximumTextFieldHeight = 48;
            var candidates = new List<ScreenRectangle>();

            for (var top = 0; top <= Height - minimumTextFieldHeight; top++)
            {
                foreach (var (left, width) in FindNeutralBrightRuns(top))
                {
                    if (width < minimumTextFieldWidth)
                        continue;

                    for (var height = minimumTextFieldHeight; height <= maximumTextFieldHeight && top + height <= Height; height++)
                    {
                        var candidate = new ScreenRectangle(left, top, width, height);

                        if (!HasTextFieldInterior(candidate))
                            continue;

                        candidates.Add(candidate);
                        break;
                    }
                }
            }

            var textFields = new List<ScreenRectangle>();

            foreach (var candidate in candidates.OrderBy(field => field.Top).ThenBy(field => field.Left))
            {
                if (textFields.Any(field => AreDuplicateDetections(field, candidate)))
                    continue;

                textFields.Add(candidate);
            }

            return textFields;
        }

        private bool HasTextFieldInterior(ScreenRectangle candidate)
        {
            var bottomBorderPixels = 0;
            var darkInteriorPixels = 0;
            var interiorSampleCount = 0;

            for (var x = candidate.Left; x < candidate.Right; x++)
            {
                if (IsNeutralAndBright(GetPixel(x, candidate.Bottom - 1)))
                    bottomBorderPixels++;
            }

            for (var y = candidate.Top + 3; y < candidate.Bottom - 3; y += 2)
            {
                for (var x = candidate.Left + 6; x < candidate.Right - 6; x += 4)
                {
                    interiorSampleCount++;

                    if (GetBrightness(GetPixel(x, y)) <= 20)
                        darkInteriorPixels++;
                }
            }

            return (double)bottomBorderPixels / candidate.Width >= 0.8
                && interiorSampleCount > 0
                && (double)darkInteriorPixels / interiorSampleCount >= 0.8;
        }

        private PixelColor GetPixel(int x, int y)
        {
            var offset = _pixelDataOffset + (y * Width + x) * 3;
            return new PixelColor(_imageBytes[offset], _imageBytes[offset + 1], _imageBytes[offset + 2]);
        }

        private bool IsHorizontallyCentered(ScreenRectangle rectangle)
        {
            return Math.Abs(rectangle.Left + rectangle.Width / 2 - Width / 2) <= 5;
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

        private static bool HaveMatchingWidths(ScreenRectangle first, ScreenRectangle second)
        {
            return Math.Abs(first.Width - second.Width) <= 5;
        }

        private static bool HaveStandardVerticalSpacing(ScreenRectangle upper, ScreenRectangle lower)
        {
            var spacing = lower.Top - upper.Bottom;
            return spacing is >= 5 and <= 12;
        }

        private static bool AreDuplicateDetections(ScreenRectangle first, ScreenRectangle second)
        {
            return Math.Abs(first.Left - second.Left) <= 3
                && Math.Abs(first.Top - second.Top) <= 3
                && Math.Abs(first.Right - second.Right) <= 3
                && Math.Abs(first.Bottom - second.Bottom) <= 3;
        }

        private static bool IsBlack(PixelColor pixel)
        {
            return pixel.Red <= BlackThreshold && pixel.Green <= BlackThreshold && pixel.Blue <= BlackThreshold;
        }

        private static bool IsNeutral(PixelColor pixel)
        {
            var minimum = Math.Min(pixel.Red, Math.Min(pixel.Green, pixel.Blue));
            var maximum = Math.Max(pixel.Red, Math.Max(pixel.Green, pixel.Blue));
            return maximum - minimum <= NeutralColorTolerance;
        }

        private static bool IsNeutralAndBright(PixelColor pixel)
        {
            var brightness = GetBrightness(pixel);
            return IsNeutral(pixel) && brightness >= 80;
        }

        private static byte GetBrightness(PixelColor pixel)
        {
            return (byte)((pixel.Red + pixel.Green + pixel.Blue) / 3);
        }
    }

    readonly record struct PixelColor(byte Red, byte Green, byte Blue);

    readonly record struct ScreenRectangle(int Left, int Top, int Width, int Height)
    {
        public int Right => Left + Width;
        public int Bottom => Top + Height;
        public int CenterX => Left + Width / 2;
        public int CenterY => Top + Height / 2;

        public ScreenRectangle Inset(int amount)
        {
            if (amount < 0 || Width <= amount * 2 || Height <= amount * 2)
                throw new ArgumentOutOfRangeException(nameof(amount));

            return new ScreenRectangle(Left + amount, Top + amount, Width - amount * 2, Height - amount * 2);
        }

        public override string ToString()
        {
            return $"({Left},{Top}) {Width}x{Height}";
        }
    }

    sealed record ConnectionScreenObservation(ConnectionNavigationKind Kind, ScreenRectangle InteractionArea, ScreenRectangle? ServerAddressField);

}
