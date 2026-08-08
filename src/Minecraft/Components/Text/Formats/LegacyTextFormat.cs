using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Components.Text.Properties;

namespace Void.Minecraft.Components.Text.Formats;

/// <summary>Describes a legacy Minecraft text formatting control code.</summary>
public record LegacyTextFormat
{
    private static readonly List<LegacyTextFormat> _set = [];

    /// <summary>Gets the character following the legacy section-sign prefix.</summary>
    public char Code { get; }
    /// <summary>Gets the canonical format name.</summary>
    public string Name { get; }
    /// <summary>Gets the operation that applies this format to existing formatting.</summary>
    public Func<Formatting, Formatting> Transform { get; }

    /// <summary>Creates and registers a legacy text format.</summary>
    /// <param name="code">The character following the legacy section-sign prefix.</param>
    /// <param name="name">The canonical format or color name.</param>
    /// <remarks>Unknown codes produce a transform that resets formatting to <see cref="Formatting.Default"/>.</remarks>
    public LegacyTextFormat(char code, string name)
    {
        Code = code;
        Name = name;
        Transform = code switch
        {
            >= '0' and <= '9' or >= 'a' and <= 'f' => formatting => formatting with { Color = name },
            'k' => formatting => formatting with { IsObfuscated = true },
            'l' => formatting => formatting with { IsBold = true },
            'm' => formatting => formatting with { IsStrikethrough = true },
            'n' => formatting => formatting with { IsUnderlined = true },
            'o' => formatting => formatting with { IsItalic = true },
            'r' or _ => _ => Formatting.Default,
        };

        _set.Add(this);
    }

    /// <summary>Gets the black color format, identified by code <c>0</c>.</summary>
    public static LegacyTextFormat Black { get; } = new('0', "black");
    /// <summary>Gets the dark blue color format, identified by code <c>1</c>.</summary>
    public static LegacyTextFormat DarkBlue { get; } = new('1', "dark_blue");
    /// <summary>Gets the dark green color format, identified by code <c>2</c>.</summary>
    public static LegacyTextFormat DarkGreen { get; } = new('2', "dark_green");
    /// <summary>Gets the dark aqua color format, identified by code <c>3</c>.</summary>
    public static LegacyTextFormat DarkAqua { get; } = new('3', "dark_aqua");
    /// <summary>Gets the dark red color format, identified by code <c>4</c>.</summary>
    public static LegacyTextFormat DarkRed { get; } = new('4', "dark_red");
    /// <summary>Gets the dark purple color format, identified by code <c>5</c>.</summary>
    public static LegacyTextFormat DarkPurple { get; } = new('5', "dark_purple");
    /// <summary>Gets the gold color format, identified by code <c>6</c>.</summary>
    public static LegacyTextFormat Gold { get; } = new('6', "gold");
    /// <summary>Gets the gray color format, identified by code <c>7</c>.</summary>
    public static LegacyTextFormat Gray { get; } = new('7', "gray");
    /// <summary>Gets the dark gray color format, identified by code <c>8</c>.</summary>
    public static LegacyTextFormat DarkGray { get; } = new('8', "dark_gray");
    /// <summary>Gets the blue color format, identified by code <c>9</c>.</summary>
    public static LegacyTextFormat Blue { get; } = new('9', "blue");
    /// <summary>Gets the green color format, identified by code <c>a</c>.</summary>
    public static LegacyTextFormat Green { get; } = new('a', "green");
    /// <summary>The legacy text format for aqua, identified by the <c>b</c> control code.</summary>
    public static LegacyTextFormat Aqua { get; } = new('b', "aqua");
    /// <summary>Gets the red color format, identified by code <c>c</c>.</summary>
    public static LegacyTextFormat Red { get; } = new('c', "red");
    /// <summary>Gets the light purple color format, identified by code <c>d</c>.</summary>
    public static LegacyTextFormat LightPurple { get; } = new('d', "light_purple");
    /// <summary>Gets the yellow color format, identified by code <c>e</c>.</summary>
    public static LegacyTextFormat Yellow { get; } = new('e', "yellow");
    /// <summary>Gets the white color format, identified by code <c>f</c>.</summary>
    public static LegacyTextFormat White { get; } = new('f', "white");

    /// <summary>Gets the obfuscated-text format, identified by code <c>k</c>.</summary>
    public static LegacyTextFormat Obfuscated { get; } = new('k', "obfuscated");
    /// <summary>Gets the bold format, identified by code <c>l</c>.</summary>
    public static LegacyTextFormat Bold { get; } = new('l', "bold");
    /// <summary>Gets the strikethrough format, identified by code <c>m</c>.</summary>
    public static LegacyTextFormat Strikethrough { get; } = new('m', "strikethrough");
    /// <summary>Gets the underlined format, identified by code <c>n</c>.</summary>
    public static LegacyTextFormat Underlined { get; } = new('n', "underlined");
    /// <summary>Gets the italic format, identified by code <c>o</c>.</summary>
    public static LegacyTextFormat Italic { get; } = new('o', "italic");

    /// <summary>Gets the reset format, identified by code <c>r</c>.</summary>
    public static LegacyTextFormat Reset { get; } = new('r', "reset");

    /// <summary>Finds a registered legacy format by its control-code character.</summary>
    /// <param name="code">The character following the section-sign prefix.</param>
    /// <param name="result">The matching format when found; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when a matching format is registered; otherwise <see langword="false"/>.</returns>
    public static bool TryFromCode(char code, [MaybeNullWhen(false)] out LegacyTextFormat result)
    {
        result = _set.Find(format => format.Code == code);
        return result is not null;
    }

    /// <summary>Finds a registered legacy format by its case-sensitive canonical name.</summary>
    /// <param name="name">The name to match.</param>
    /// <param name="result">The matching format when found; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when a matching format is registered; otherwise <see langword="false"/>.</returns>
    public static bool TryFromName(ReadOnlySpan<char> name, [MaybeNullWhen(false)] out LegacyTextFormat result)
    {
        foreach (var format in _set)
        {
            if (format.Name.AsSpan().SequenceEqual(name))
            {
                result = format;
                return true;
            }
        }

        result = null;
        return false;
    }
}
