using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record DoubleProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<DoubleProperty>
{
    public double AsPrimitive => new MinecraftBuffer(Value.Span).ReadDouble();

    public static DoubleProperty FromPrimitive(double value)
    {
        Span<byte> bytes = stackalloc byte[sizeof(double)];
        var buffer = new MinecraftBuffer(bytes);
        buffer.WriteDouble(value);
        return new DoubleProperty(bytes.ToArray());
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
