using Void.Proxy.API.Network.IO;

namespace Void.Proxy.API.Network.Protocol.Packets;

public interface IMinecraftPacket
{
    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
}