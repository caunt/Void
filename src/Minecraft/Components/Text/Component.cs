using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Void.Minecraft.Components.Text.Formats;
using Void.Minecraft.Components.Text.Properties;
using Void.Minecraft.Components.Text.Properties.Content;

namespace Void.Minecraft.Components.Text;

public record Component(IContent Content, Children Children, Formatting Formatting, Interactivity Interactivity)
{
    public const string ExampleLegacyString = "&1Hello, &2this is a &x&F&F&A&A&0&0hex colored " +
                                  "text, &lwith bold, &oitalic, &nunderline, &mstrikethrough, " +
                                  "&kobfuscated, &rand reset.";

    public static Component Default { get; } = new(new TextContent(string.Empty), Children.Default, Formatting.Default, Interactivity.Default);

    public static Component DeserializeLegacy(string source)
    {
        var span = source.AsSpan();
        var segments = new List<Component>();
        var text = new StringBuilder();

        var formatting = Formatting.Default;

        for (var i = 0; i < span.Length; i++)
        {
            var prefix = span[i];
            if (prefix != '&' || i + 1 >= span.Length)
            {
                text.Append(span[i]);
                continue;
            }

            if (text.Length > 0)
                MoveNext();

            i++; // move to code
            var code = span[i];

            if (code is 'x')
            {
                if (TryParseHexSequence(span, ref i, out var hex))
                    formatting = formatting with { Color = "#" + hex };
                else
                    text.Append("&x");

                continue;
            }
            else if (code is 'r')
            {
                formatting = Formatting.Default;
                continue;
            }
            else if (LegacyTextFormat.TryFromCode(code, out var format))
            {
                formatting = format.Transform(formatting);
            }
            else
            {
                text.Append("&" + code);
            }
        }

        if (text.Length > 0)
            MoveNext();

        return Default with { Children = new(segments) };

        void MoveNext()
        {
            var content = new TextContent(text.ToString());
            text.Clear();

            segments.Add(Default with
            {
                Content = content,
                Formatting = formatting
            });
        }
    }
    private static bool TryParseHexSequence(ReadOnlySpan<char> source, ref int sourceIndex, [MaybeNullWhen(false)] out string result)
    {
        var length = 6 * 2; // & before each digit
        result = null;

        if (sourceIndex + length - 1 >= source.Length)
            return false;

        var builder = new StringBuilder();

        for (var index = sourceIndex + 1; index < sourceIndex + length; index++)
        {
            var digit = source[index + 1];

            if (source[index] == '&' && IsHexDigit(digit))
            {
                index++;
                builder.Append(digit);
            }
            else
            {
                break;
            }
        }

        result = builder.ToString();
        sourceIndex += length;

        return true;
    }

    private static bool IsHexDigit(char c) => c is
            >= '0' and <= '9' or
            >= 'a' and <= 'f' or
            >= 'A' and <= 'F';
}