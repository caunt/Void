using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.API.Network.IO.Messages;

public interface IMinecraftPacket : IMinecraftMessage
{
    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
}