using System;
using System.IO;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record IntProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<IntProperty>
{
    public int AsPrimitive => new MinecraftBuffer(Value.Span).ReadInt();

    public static IntProperty FromPrimitive(int value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteInt(value);

        return new IntProperty(stream.ToArray());
    }

    public static IntProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadInt());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteInt(AsPrimitive);
    }
}
