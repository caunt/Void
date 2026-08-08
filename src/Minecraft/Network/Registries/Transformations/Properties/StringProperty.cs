using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>
/// Represents a variable-length-prefixed UTF-8 string packet property.
/// </summary>
/// <param name="Value">The encoded length prefix and UTF-8 bytes, retained without copying or validation.</param>
public record StringProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<StringProperty>
{
    /// <summary>Gets the string decoded from the property bytes.</summary>
    public string AsPrimitive => new MinecraftBuffer(Value.Span).ReadString();

    /// <summary>Gets the decoded string parsed as a new JSON node.</summary>
    public JsonNode AsJsonNode => ToJsonNode();

    /// <summary>
    /// Serializes a JSON node to compact text by default and encodes that text as a string property.
    /// </summary>
    /// <param name="value">The JSON node to serialize.</param>
    /// <param name="jsonSerializerOptions">Optional JSON serializer options; <see langword="null" /> selects non-indented output.</param>
    /// <returns>The encoded string property.</returns>
    public static StringProperty FromJsonNode(JsonNode value, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        return FromPrimitive(value.ToJsonString(jsonSerializerOptions ?? new JsonSerializerOptions { WriteIndented = false }));
    }

    /// <summary>
    /// Encodes text as a variable-length-prefixed UTF-8 string property.
    /// </summary>
    /// <param name="value">The characters to encode.</param>
    /// <returns>The encoded string property.</returns>
    public static StringProperty FromPrimitive(ReadOnlySpan<char> value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteString(value);

        return new StringProperty(stream.GetBuffer().AsMemory(0, (int)stream.Length));
    }

    /// <summary>
    /// Reads one protocol string and stores its normalized encoded representation.
    /// </summary>
    /// <param name="buffer">The source buffer.</param>
    /// <returns>The decoded property.</returns>
    public static StringProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadString());
    }

    /// <summary>
    /// Parses the decoded string as JSON.
    /// </summary>
    /// <param name="jsonNodeOptions">Options controlling node creation.</param>
    /// <param name="jsonDocumentOptions">Options controlling JSON parsing.</param>
    /// <returns>The parsed JSON node.</returns>
    /// <exception cref="InvalidDataException">Parsing returns a null node.</exception>
    /// <exception cref="JsonException">The decoded string is not valid JSON under the supplied options.</exception>
    public JsonNode ToJsonNode(JsonNodeOptions? jsonNodeOptions = null, JsonDocumentOptions jsonDocumentOptions = default)
    {
        return JsonNode.Parse(AsPrimitive, jsonNodeOptions, jsonDocumentOptions) ?? throw new InvalidDataException($"Failed to parse {nameof(JsonNode)}: {AsPrimitive}");
    }

    /// <summary>Writes the decoded text as a protocol string.</summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteString(AsPrimitive);
    }
}
