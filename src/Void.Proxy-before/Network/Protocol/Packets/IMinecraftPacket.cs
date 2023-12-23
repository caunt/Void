using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.States;

namespace Void.Proxy.Network.Protocol.Packets;

public interface IMinecraftPacket<in T> : IMinecraftPacket where T : IProtocolState
{
    Task<bool> HandleAsync(T state);
}

public interface IMinecraftPacket
{
    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);

    public int MaxSize() => 256;
}