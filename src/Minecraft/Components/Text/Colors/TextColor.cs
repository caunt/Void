using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace Void.Minecraft.Components.Text.Colors;

public record TextColor(byte Red, byte Green, byte Blue)
{
    private static readonly Dictionary<(byte, byte, byte), string> _map = new()
    {
        [(0x00, 0x00, 0x00)] = "black",
        [(0x00, 0x00, 0xAA)] = "dark_blue",
        [(0x00, 0xAA, 0x00)] = "dark_green",
        [(0x00, 0xAA, 0xAA)] = "dark_aqua",
        [(0xAA, 0x00, 0x00)] = "dark_red",
        [(0xAA, 0x00, 0xAA)] = "dark_purple",
        [(0xFF, 0xAA, 0x00)] = "gold",
        [(0xAA, 0xAA, 0xAA)] = "gray",
        [(0x55, 0x55, 0x55)] = "dark_gray",
        [(0x55, 0x55, 0xFF)] = "blue",
        [(0x55, 0xFF, 0x55)] = "green",
        [(0x55, 0xFF, 0xFF)] = "aqua",
        [(0xFF, 0x55, 0x55)] = "red",
        [(0xFF, 0x55, 0xFF)] = "light_purple",
        [(0xFF, 0xFF, 0x55)] = "yellow",
        [(0xFF, 0xFF, 0xFF)] = "white"
    };

    public string Name => _map.TryGetValue((Red, Green, Blue), out var name) ? name : $"#{Red:X2}{Green:X2}{Blue:X2}";

    public static implicit operator TextColor((byte Red, byte Green, byte Blue) color) => new(color.Red, color.Green, color.Blue);
    public static implicit operator TextColor(Color color) => new(color.R, color.G, color.B);
    public static implicit operator TextColor(string color) => FromString(color);
    public static implicit operator Color(TextColor color) => Color.FromArgb(color.Red, color.Green, color.Blue);
    public static implicit operator string(TextColor color) => color.Name;

    public static TextColor FromString(string value)
    {
        var span = value.AsSpan();
        if (value.StartsWith('#') && value.Length == 7)
        {
            if (byte.TryParse(span[1..3], NumberStyles.HexNumber, null, out var red) && byte.TryParse(span[3..5], NumberStyles.HexNumber, null, out var green) && byte.TryParse(span[5..7], NumberStyles.HexNumber, null, out var blue))
            {
                return (red, green, blue);
            }
            else
            {
                throw new ArgumentException($"Invalid hex color string: {span}");
            }
        }

        foreach (var (rgb, name) in _map)
        {
            if (string.Equals(name, value, StringComparison.OrdinalIgnoreCase))
            {
                return rgb;
            }
        }

        throw new ArgumentException($"Invalid color string: {span}");
    }

    public override string ToString() => Name;
}
