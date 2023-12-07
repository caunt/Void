using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States;

namespace MinecraftProxy.Network.Protocol.Packets;

public interface IMinecraftPacket<in T> : IMinecraftPacket where T : IProtocolState
{
    Task<bool> HandleAsync(T state);
}

public interface IMinecraftPacket
{
    public void Decode(ref MinecraftBuffer buffer);
    public void Encode(ref MinecraftBuffer buffer);
}