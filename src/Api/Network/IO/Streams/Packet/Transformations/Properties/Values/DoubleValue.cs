using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public record DoubleValue(ReadOnlyMemory<byte> Value) : IPropertyValue
{
    public double AsPrimitive => new MinecraftBuffer(Value.Span).ReadDouble();

    public static DoubleValue FromPrimitive(double value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteDouble(value);
        
        return new DoubleValue(stream.ToArray());
    }
}
