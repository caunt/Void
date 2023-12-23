using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Network.IO;

namespace Void.Proxy.Network.Protocol.Packets;

public interface IMinecraftPacket
{
    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
}