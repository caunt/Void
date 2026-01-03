using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record StringProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<StringProperty>
{
    public string AsPrimitive => new MinecraftBuffer(Value.Span).ReadString();
    public JsonNode AsJsonNode => ToJsonNode();

    public static StringProperty FromJsonNode(JsonNode value, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        return FromPrimitive(value.ToJsonString(jsonSerializerOptions ?? new JsonSerializerOptions { WriteIndented = false }));
    }

    public static StringProperty FromPrimitive(ReadOnlySpan<char> value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteString(value);

        return new StringProperty(stream.GetBuffer().AsMemory(0, (int)stream.Length));
    }

    public static StringProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadString());
    }

    public JsonNode ToJsonNode(JsonNodeOptions? jsonNodeOptions = null, JsonDocumentOptions jsonDocumentOptions = default)
    {
        return JsonNode.Parse(AsPrimitive, jsonNodeOptions, jsonDocumentOptions) ?? throw new InvalidDataException($"Failed to parse {nameof(JsonNode)}: {AsPrimitive}");
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteString(AsPrimitive);
    }
}
