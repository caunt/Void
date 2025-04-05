using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public record ByteValue(ReadOnlyMemory<byte> Value) : IPropertyValue
{
    public byte AsPrimitive => new MinecraftBuffer(Value.Span).ReadUnsignedByte();

    public static ByteValue FromPrimitive(byte value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteUnsignedByte(value);
        
        return new ByteValue(stream.ToArray());
    }
}
