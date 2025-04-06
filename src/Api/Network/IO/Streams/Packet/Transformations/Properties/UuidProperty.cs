using Void.Minecraft.Buffers;
using Void.Minecraft.Profiles;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties;

public record UuidProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<UuidProperty>
{
    public Uuid AsUuid => new MinecraftBuffer(Value.Span).ReadUuid();

    public static UuidProperty FromUuid(Uuid value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteUuid(value);

        return new UuidProperty(stream.ToArray());
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
