using System;
using System.IO;
using System.Text.Json.Nodes;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record StringProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<StringProperty>
{
    public string AsPrimitive => new MinecraftBuffer(Value.Span).ReadString();
    public JsonNode AsJsonNode => JsonNode.Parse(AsPrimitive) ?? throw new InvalidDataException($"Failed to parse JSON string: {AsPrimitive}");

    public static StringProperty FromJsonNode(JsonNode value)
    {
        return FromPrimitive(value.GetValue<string>());
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

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteString(AsPrimitive);
    }
}
