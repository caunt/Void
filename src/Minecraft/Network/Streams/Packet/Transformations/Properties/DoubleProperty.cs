using System;
using System.IO;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Streams.Packet.Transformations.Properties;

public record DoubleProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<DoubleProperty>
{
    public double AsPrimitive => new MinecraftBuffer(Value.Span).ReadDouble();

    public static DoubleProperty FromPrimitive(double value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteDouble(value);

        return new DoubleProperty(stream.ToArray());
    }

    public static DoubleProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadDouble());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteDouble(AsPrimitive);
    }
}
