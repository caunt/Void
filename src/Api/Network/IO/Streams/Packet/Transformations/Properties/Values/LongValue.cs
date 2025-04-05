using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public record LongValue(ReadOnlyMemory<byte> Value) : IPropertyValue
{
    public long AsPrimitive => new MinecraftBuffer(Value.Span).ReadLong();

    public static LongValue FromPrimitive(long value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteLong(value);
        
        return new LongValue(stream.ToArray());
    }
}
