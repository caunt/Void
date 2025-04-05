using Void.Minecraft.Buffers;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public record BoolValue(ReadOnlyMemory<byte> Value) : IPropertyValue
{
    public bool AsPrimitive => new MinecraftBuffer(Value.Span).ReadBoolean();

    public static BoolValue FromPrimitive(bool value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteBoolean(value);
        
        return new BoolValue(stream.ToArray());
    }
}
