using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public record StringValue(ReadOnlyMemory<byte> Value) : IPropertyValue
{
    public string AsPrimitive => new MinecraftBuffer(Value.Span).ReadString();

    public static StringValue FromPrimitive(string value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteString(value);
        
        return new StringValue(stream.ToArray());
    }
}
