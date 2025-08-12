using System;
using System.IO;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record StringProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<StringProperty>
{
    public string AsPrimitive => new MinecraftBuffer(Value.Span).ReadString();

    public static StringProperty FromPrimitive(string value)
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
