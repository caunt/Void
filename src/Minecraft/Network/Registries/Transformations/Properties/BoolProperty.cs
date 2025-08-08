using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record BoolProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<BoolProperty>
{
    public bool AsPrimitive => new MinecraftBuffer(Value.Span).ReadBoolean();

    public static BoolProperty FromPrimitive(bool value)
    {
        Span<byte> data = stackalloc byte[1];
        var buffer = new MinecraftBuffer(data);
        buffer.WriteBoolean(value);

        return new BoolProperty(data.ToArray());
    }

    public static BoolProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadBoolean());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteBoolean(AsPrimitive);
    }
}
