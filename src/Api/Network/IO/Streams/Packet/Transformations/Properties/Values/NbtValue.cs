using Void.Minecraft.Buffers;
using Void.Minecraft.Nbt;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

public record NbtValue(ReadOnlyMemory<byte> Value) : IPropertyValue
{
    public NbtTag AsNbtTag => new MinecraftBuffer(Value.Span).ReadTag();

    public static NbtValue FromNbtTag(NbtTag value)
    {
        using var stream = new MemoryStream();
        var buffer = new MinecraftBuffer(stream);
        buffer.WriteTag(value);
        
        return new NbtValue(stream.ToArray());
    }
}