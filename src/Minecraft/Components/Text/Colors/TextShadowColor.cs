using System;
using System.Drawing;
using System.Globalization;

namespace Void.Minecraft.Components.Text.Colors;

/// <summary>Represents the ARGB shadow color of a Minecraft text component.</summary>
/// <param name="Alpha">The alpha channel.</param>
/// <param name="Red">The red channel.</param>
/// <param name="Green">The green channel.</param>
/// <param name="Blue">The blue channel.</param>
public record TextShadowColor(byte Alpha, byte Red, byte Green, byte Blue)
{
    /// <summary>Gets the color as an uppercase <c>#RRGGBBAA</c> string.</summary>
    public string Name => $"#{Red:X2}{Green:X2}{Blue:X2}{Alpha:X2}";

    /// <summary>Creates a shadow color from an ARGB tuple.</summary>
    /// <param name="color">The ARGB channels.</param>
    /// <returns>A color containing the supplied channels.</returns>
    public static implicit operator TextShadowColor((byte Alpha, byte Red, byte Green, byte Blue) color) => new(color.Alpha, color.Red, color.Green, color.Blue);
    /// <summary>Creates a shadow color from a drawing color.</summary>
    /// <param name="color">The drawing color.</param>
    /// <returns>The corresponding shadow color.</returns>
    public static implicit operator TextShadowColor(Color color) => new(color.A, color.R, color.G, color.B);
    /// <summary>Parses a hexadecimal shadow color.</summary>
    /// <param name="color">A <c>#RRGGBBAA</c> value.</param>
    /// <returns>The parsed shadow color.</returns>
    /// <exception cref="ArgumentException"><paramref name="color"/> is not a valid hexadecimal shadow color.</exception>
    public static implicit operator TextShadowColor(string color) => FromString(color);
    /// <summary>Converts a shadow color to a drawing color.</summary>
    /// <param name="color">The shadow color.</param>
    /// <returns>The corresponding drawing color.</returns>
    public static implicit operator Color(TextShadowColor color) => Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
    /// <summary>Converts a shadow color to its <c>#RRGGBBAA</c> representation.</summary>
    /// <param name="color">The shadow color.</param>
    /// <returns>The value of <see cref="Name"/>.</returns>
    public static implicit operator string(TextShadowColor color) => color.Name;
    /// <summary>Packs a shadow color into a signed 32-bit ARGB value.</summary>
    /// <param name="color">The shadow color.</param>
    /// <returns>The packed ARGB value.</returns>
    public static implicit operator int(TextShadowColor color) => (color.Alpha << 24) + (color.Red << 16) + (color.Green << 8) + color.Blue;
    /// <summary>Unpacks a signed 32-bit ARGB value into a shadow color.</summary>
    /// <param name="value">The packed ARGB value.</param>
    /// <returns>The unpacked shadow color.</returns>
    public static implicit operator TextShadowColor(int value) => new((byte)(value >> 24), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF));
    /// <summary>Converts the channel bytes to normalized floating-point components.</summary>
    /// <param name="color">The shadow color.</param>
    /// <returns>An array containing the implementation's alpha, red, green, and blue component calculations.</returns>
    public static implicit operator float[](TextShadowColor color) => [(color.Alpha >> 24 & 0xFF) / 255f, (color.Red >> 16 & 0xFF) / 255f, (color.Green >> 8 & 0xFF) / 255f, (color.Blue & 0xFF) / 255f];
    /// <summary>Creates a shadow color from normalized floating-point components.</summary>
    /// <param name="components">Alpha, red, green, and blue components in that order.</param>
    /// <returns>The color produced by multiplying each component by 255 and converting it to a byte.</returns>
    /// <exception cref="IndexOutOfRangeException"><paramref name="components"/> contains fewer than four elements.</exception>
    public static implicit operator TextShadowColor(float[] components) => new((byte)(components[0] * 255), (byte)(components[1] * 255), (byte)(components[2] * 255), (byte)(components[3] * 255));

    /// <summary>Parses a hexadecimal RGBA shadow color.</summary>
    /// <param name="value">A <c>#RRGGBBAA</c> value.</param>
    /// <returns>The parsed shadow color.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> is not a valid hexadecimal shadow color.</exception>
    public static TextShadowColor FromString(string value)
    {
        var span = value.AsSpan();
        if (span.Length == 9 && span[0] == '#')
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

    /// <summary>Returns the uppercase <c>#RRGGBBAA</c> representation of this color.</summary>
    /// <returns>The value of <see cref="Name"/>.</returns>
    public override string ToString() => Name;
}
