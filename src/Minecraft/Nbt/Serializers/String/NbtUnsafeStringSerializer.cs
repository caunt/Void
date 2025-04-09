using System.Text.RegularExpressions;
using Void.Minecraft.Nbt.Serializers.Json;

namespace Void.Minecraft.Nbt.Serializers.String;

public static partial class NbtUnsafeStringSerializer
{
    public static string Serialize(NbtTag tag)
    {
        return tag.ToString();
    }

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
