using System;
using System.IO;
using Void.Minecraft.Buffers;
using Void.Minecraft.Nbt;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record NbtProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<NbtProperty>
{
    public NbtTag AsNbtTag => new MinecraftBuffer(Value.Span).ReadTag();

    public static NbtProperty FromNbtTag(NbtTag value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteTag(value);

        return new NbtProperty(stream.GetBuffer().AsMemory(0, (int)stream.Length));
    }

    public static NbtProperty Read(ref MinecraftBuffer buffer)
    {
        return FromNbtTag(buffer.ReadTag());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteTag(AsNbtTag);
    }
}
