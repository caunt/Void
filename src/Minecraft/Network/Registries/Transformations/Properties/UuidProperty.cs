using System;
using Void.Minecraft.Buffers;
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record UuidProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<UuidProperty>
{
    public static UuidProperty Empty { get; } = FromUuid(Uuid.Empty);

    public Uuid AsUuid => new MinecraftBuffer(Value.Span).ReadUuid();

    public static UuidProperty FromUuid(Uuid value)
    {
        var bytes = new byte[16];
        var buffer = new MinecraftBuffer(bytes.AsSpan());
        buffer.WriteUuid(value);

        return new UuidProperty(bytes);
    }

    public static UuidProperty Read(ref MinecraftBuffer buffer)
    {
        return FromUuid(buffer.ReadUuid());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteUuid(AsUuid);
    }
}
