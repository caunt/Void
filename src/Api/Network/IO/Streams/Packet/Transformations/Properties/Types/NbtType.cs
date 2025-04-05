using Void.Minecraft.Buffers;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;

namespace Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Types;

public record NbtType : IPropertyType<NbtValue>
{
    public NbtValue Read(ref MinecraftBuffer buffer)
    {
        return NbtValue.FromNbtTag(buffer.ReadTag());
    }

    public void Write(ref MinecraftBuffer buffer, NbtValue value)
    {
        buffer.WriteTag(value.AsNbtTag);
    }
}
