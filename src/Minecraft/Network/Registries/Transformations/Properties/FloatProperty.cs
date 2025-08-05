using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record FloatProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<FloatProperty>
{
    public float AsPrimitive => new MinecraftBuffer(Value.Span).ReadFloat();

    public static FloatProperty FromPrimitive(float value)
    {
        var bytes = new byte[4];
        var buffer = new MinecraftBuffer(bytes);
        buffer.WriteFloat(value);

        return new FloatProperty(bytes);
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
