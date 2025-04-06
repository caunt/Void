using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties;

public record LongProperty(ReadOnlyMemory<byte> Value) : IPacketProperty
{
    public long AsPrimitive => new MinecraftBuffer(Value.Span).ReadLong();

    public static LongProperty FromPrimitive(long value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteLong(value);

        return new LongProperty(stream.ToArray());
    }
    public static LongProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadLong());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteLong(AsPrimitive);
    }
}
