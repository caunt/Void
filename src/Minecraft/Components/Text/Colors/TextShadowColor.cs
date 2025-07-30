using System;
using System.Drawing;
using System.Globalization;

namespace Void.Minecraft.Components.Text.Colors;

public record TextShadowColor(byte Alpha, byte Red, byte Green, byte Blue)
{
    public string Name => $"#{Red:X2}{Green:X2}{Blue:X2}{Alpha:X2}";

    public static implicit operator TextShadowColor((byte Alpha, byte Red, byte Green, byte Blue) color) => new(color.Alpha, color.Red, color.Green, color.Blue);
    public static implicit operator TextShadowColor(Color color) => new(color.A, color.R, color.G, color.B);
    public static implicit operator TextShadowColor(string color) => FromString(color);
    public static implicit operator Color(TextShadowColor color) => Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
    public static implicit operator string(TextShadowColor color) => color.Name;
    public static implicit operator int(TextShadowColor color) => (color.Alpha << 24) + (color.Red << 16) + (color.Green << 8) + color.Blue;
    public static implicit operator TextShadowColor(int value) => new((byte)(value >> 24), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF));
    public static implicit operator float[](TextShadowColor color) => [(color.Alpha >> 24 & 0xFF) / 255f, (color.Red >> 16 & 0xFF) / 255f, (color.Green >> 8 & 0xFF) / 255f, (color.Blue & 0xFF) / 255f];
    public static implicit operator TextShadowColor(float[] components) => new((byte)(components[0] * 255), (byte)(components[1] * 255), (byte)(components[2] * 255), (byte)(components[3] * 255));

    public static TextShadowColor FromString(string value)
    {
        var span = value.AsSpan();
        if (value.StartsWith('#') && value.Length == 9)
        {
            if (byte.TryParse(span[1..3], NumberStyles.HexNumber, null, out var red) && byte.TryParse(span[3..5], NumberStyles.HexNumber, null, out var green) && byte.TryParse(span[5..7], NumberStyles.HexNumber, null, out var blue) && byte.TryParse(span[7..9], NumberStyles.HexNumber, null, out var alpha))
            {
                return (alpha, red, green, blue);
            }
            else
            {
                throw new ArgumentException($"Invalid hex color string: {span}");
            }
        }

        throw new ArgumentException($"Invalid color string: {span}");
    }

    public override string ToString() => Name;
}
