using System;
using System.IO;
using Void.Minecraft.Buffers;
using Void.Minecraft.Nbt;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record NamedNbtProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<NamedNbtProperty>
{
    public NbtTag AsNbtTag => new MinecraftBuffer(Value.Span).ReadTag(readName: true);

    public static NamedNbtProperty FromNbtTag(NbtTag value)
    {
        value.Name ??= string.Empty;

        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteTag(value, writeName: true);

        return new NamedNbtProperty(stream.GetBuffer().AsMemory(0, (int)stream.Length));
    }

    public static NamedNbtProperty Read(ref MinecraftBuffer buffer)
    {
        return FromNbtTag(buffer.ReadTag(readName: true));
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteTag(AsNbtTag);
    }
}
