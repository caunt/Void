using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public record ShortValue(ReadOnlyMemory<byte> Value) : IPropertyValue
{
    public ushort AsPrimitive => new MinecraftBuffer(Value.Span).ReadUnsignedShort();

    public static ShortValue FromPrimitive(ushort value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteUnsignedShort(value);
        
        return new ShortValue(stream.ToArray());
    }
}
