using System.Text.RegularExpressions;
using Void.Minecraft.Nbt.Serializers.Json;

namespace Void.Minecraft.Nbt.Serializers.String;

/// <summary>Provides a permissive, JSON-assisted parser for a limited subset of SNBT.</summary>
/// <remarks>The parser only quotes word-character keys and converts simple single-quoted values; it is not a complete SNBT parser.</remarks>
public static partial class NbtUnsafeStringSerializer
{
    /// <summary>Serializes a tag using the standard SNBT serializer.</summary>
    /// <param name="tag">The tag to serialize.</param>
    /// <returns>The SNBT representation.</returns>
    public static string Serialize(NbtTag tag)
    {
        return NbtStringSerializer.Serialize(tag);
    }

    /// <summary>Converts supported SNBT-like syntax to JSON and infers an NBT tag from it.</summary>
    /// <param name="value">The permissive SNBT-like input.</param>
    /// <returns>The inferred NBT tag.</returns>
    public static NbtTag Deserialize(string value)
    {
        return NbtJsonSerializer.Deserialize(ConvertSnbtToJson(value));
    }

    private static string ConvertSnbtToJson(string input)
    {
        // Step 1: Add quotes around keys (match keys following { or ,).
        input = StringNbtPropertyNamePattern().Replace(input, "$1\"$2\":");

        // Step 2: Replace single-quoted strings by matching content between single quotes,
        // and then escape any double quotes inside that content.
        input = StringNbtQuotedValuesPattern().Replace(input, match =>
        {
            // Escape inner double quotes
            var escapedContent = match.Groups[1].Value.Replace("\"", "\\\"");
            return $"\"{escapedContent}\"";
        });

        return input;
    }

    [GeneratedRegex(@"([{,])\s*(\w+)\s*:")]
    private static partial Regex StringNbtPropertyNamePattern();
    [GeneratedRegex(@"'([^']*)'")]
    private static partial Regex StringNbtQuotedValuesPattern();
}
