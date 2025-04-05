using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public record IntValue(ReadOnlyMemory<byte> Value) : IPropertyValue
{
    public int AsPrimitive => new MinecraftBuffer(Value.Span).ReadInt();

    public static IntValue FromPrimitive(int value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteInt(value);
        
        return new IntValue(stream.ToArray());
    }
}
