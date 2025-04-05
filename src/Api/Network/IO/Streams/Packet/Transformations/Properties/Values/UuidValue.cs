using Void.Minecraft.Buffers;
using Void.Minecraft.Profiles;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public record UuidValue(ReadOnlyMemory<byte> Value) : IPropertyValue
{
    public Uuid AsPrimitive => new MinecraftBuffer(Value.Span).ReadUuid();

    public static UuidValue FromPrimitive(Uuid value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteUuid(value);
        
        return new UuidValue(stream.ToArray());
    }
}
