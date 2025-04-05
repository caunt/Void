using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public record FloatValue(ReadOnlyMemory<byte> Value) : IPropertyValue
{
    public float AsPrimitive => new MinecraftBuffer(Value.Span).ReadFloat();

    public static FloatValue FromPrimitive(float value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteFloat(value);
        
        return new FloatValue(stream.ToArray());
    }
}
