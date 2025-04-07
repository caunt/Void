using System;
using System.IO;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record BoolProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<BoolProperty>
{
    public bool AsPrimitive => new MinecraftBuffer(Value.Span).ReadBoolean();

    public static BoolProperty FromPrimitive(bool value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteBoolean(value);

        return new BoolProperty(stream.ToArray());
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
