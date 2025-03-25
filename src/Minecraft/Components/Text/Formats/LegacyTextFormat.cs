using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Components.Text.Properties;

namespace Void.Minecraft.Components.Text.Formats;

public record LegacyTextFormat
{
    private static readonly List<LegacyTextFormat> _set = [];

    public char Code { get; }
    public string Name { get; }
    public Func<Formatting, Formatting> Transform { get; }

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

    public static LegacyTextFormat Black { get; } = new('0', "black");
    public static LegacyTextFormat DarkBlue { get; } = new('1', "dark_blue");
    public static LegacyTextFormat DarkGreen { get; } = new('2', "dark_green");
    public static LegacyTextFormat DarkAqua { get; } = new('3', "dark_aqua");
    public static LegacyTextFormat DarkRed { get; } = new('4', "dark_red");
    public static LegacyTextFormat DarkPurple { get; } = new('5', "dark_purple");
    public static LegacyTextFormat Gold { get; } = new('6', "gold");
    public static LegacyTextFormat Gray { get; } = new('7', "gray");
    public static LegacyTextFormat DarkGray { get; } = new('8', "dark_gray");
    public static LegacyTextFormat Blue { get; } = new('9', "blue");
    public static LegacyTextFormat Green { get; } = new('a', "green");
    public static LegacyTextFormat Aqua { get; } = new('b', "aqua");
    public static LegacyTextFormat Red { get; } = new('c', "red");
    public static LegacyTextFormat LightPurple { get; } = new('d', "light_purple");
    public static LegacyTextFormat Yellow { get; } = new('e', "yellow");
    public static LegacyTextFormat White { get; } = new('f', "white");

    public static LegacyTextFormat Obfuscated { get; } = new('k', "obfuscated");
    public static LegacyTextFormat Bold { get; } = new('l', "bold");
    public static LegacyTextFormat Strikethrough { get; } = new('m', "strikethrough");
    public static LegacyTextFormat Underlined { get; } = new('n', "underlined");
    public static LegacyTextFormat Italic { get; } = new('o', "italic");

    public static LegacyTextFormat Reset { get; } = new('r', "reset");

    public static bool TryFromCode(char code, [MaybeNullWhen(false)] out LegacyTextFormat result)
    {
        result = _set.Find(format => format.Code == code);
        return result is not null;
    }

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