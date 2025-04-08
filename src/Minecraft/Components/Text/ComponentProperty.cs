using System;
using System.IO;
using Void.Minecraft.Buffers;
using Void.Minecraft.Nbt;
using Void.Minecraft.Network.Registries.Transformations.Properties;

namespace Void.Minecraft.Components.Text;

public record ComponentProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<ComponentProperty>
{
    public NbtTag AsNbtTag => new MinecraftBuffer(Value.Span).ReadTag();

    public static ComponentProperty FromNbtTag(NbtTag value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteTag(value);

        return new ComponentProperty(stream.ToArray());
    }

    public static ComponentProperty Read(ref MinecraftBuffer buffer)
    {
        return FromNbtTag(buffer.ReadTag());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteTag(AsNbtTag);
    }
}
