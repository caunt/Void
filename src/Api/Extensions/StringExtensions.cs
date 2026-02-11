using System.Text.RegularExpressions;

namespace Void.Proxy.Api.Extensions;

public static partial class StringExtensions
{
    private static readonly char[] DefaultDelimiters = [',', ';', '\n'];

    /// <summary>
    /// Splits a string by multiple delimiters with optional escape character support.
    /// </summary>
    /// <param name="input">The string to split.</param>
    /// <param name="delimiters">Custom delimiters to use. If null, uses default delimiters: comma, semicolon, and newline.</param>
    /// <param name="escapeCharacter">Optional escape character (e.g., '\') to allow escaped delimiters in the input.</param>
    /// <param name="removeEmptyEntries">Whether to remove empty entries from the result. Defaults to true.</param>
    /// <returns>An array of strings split by the delimiters.</returns>
    public static string[] SplitByDelimiters(this string input, char[]? delimiters = null, char? escapeCharacter = null, bool removeEmptyEntries = true)
    {
        if (string.IsNullOrWhiteSpace(input))
            return [];

        delimiters ??= DefaultDelimiters;

        if (escapeCharacter is not null)
        {
            return SplitByDelimitersWithEscape(input, delimiters, escapeCharacter.Value, removeEmptyEntries);
        }

        var options = removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries : StringSplitOptions.TrimEntries;
        return input.Split(delimiters, options);
    }

    private static string[] SplitByDelimitersWithEscape(string input, char[] delimiters, char escapeCharacter, bool removeEmptyEntries)
    {
        var pattern = BuildEscapedDelimiterPattern(delimiters, escapeCharacter);
        var regex = new Regex(pattern, RegexOptions.Compiled);
        var parts = regex.Split(input);

        var result = parts.Select(part => UnescapeDelimiters(part, delimiters, escapeCharacter).Trim()).ToArray();

        if (removeEmptyEntries)
            result = result.Where(part => !string.IsNullOrWhiteSpace(part)).ToArray();

        return result;
    }

    private static string BuildEscapedDelimiterPattern(char[] delimiters, char escapeCharacter)
    {
        var escapedChar = Regex.Escape(escapeCharacter.ToString());
        var delimiterPattern = string.Join("|", delimiters.Select(d => Regex.Escape(d.ToString())));
        return $"(?<!{escapedChar})(?:{delimiterPattern})";
    }

    private static string UnescapeDelimiters(string input, char[] delimiters, char escapeCharacter)
    {
        foreach (var delimiter in delimiters)
        {
            input = input.Replace($"{escapeCharacter}{delimiter}", delimiter.ToString());
        }

        return input;
    }
}
