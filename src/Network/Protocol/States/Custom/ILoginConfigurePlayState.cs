using MinecraftProxy.Network.Protocol.Packets.Clientbound;

namespace MinecraftProxy.Network.Protocol.States.Custom;

public interface ILoginConfigurePlayState : IProtocolState
{
    public Task<bool> HandleAsync(DisconnectPacket packet);
}