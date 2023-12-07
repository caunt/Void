using MinecraftProxy.Network.Protocol.Packets.Clientbound;

namespace MinecraftProxy.Network.Protocol.States.Custom;

public interface IPlayableState : IProtocolState
{
    public Task<bool> HandleAsync(DisconnectPacket packet);
}