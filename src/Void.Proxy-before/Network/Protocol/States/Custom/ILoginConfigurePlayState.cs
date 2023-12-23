using Void.Proxy.Network.Protocol.Packets.Clientbound;

namespace Void.Proxy.Network.Protocol.States.Custom;

public interface ILoginConfigurePlayState : IProtocolState
{
    public Task<bool> HandleAsync(DisconnectPacket packet);
}