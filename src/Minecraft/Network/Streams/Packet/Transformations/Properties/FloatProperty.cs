using System;
using System.IO;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Streams.Packet.Transformations.Properties;

public record FloatProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<FloatProperty>
{
    public float AsPrimitive => new MinecraftBuffer(Value.Span).ReadFloat();

    public static FloatProperty FromPrimitive(float value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteFloat(value);

        return new FloatProperty(stream.ToArray());
    }

    public static FloatProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadFloat());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteFloat(AsPrimitive);
    }
}
