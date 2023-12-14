using Void.Proxy.Network.Protocol.Packets.Shared;

namespace Void.Proxy.Network.Protocol.States.Custom;

public interface IConfigurePlayState : IProtocolState
{
    public Task<bool> HandleAsync(PluginMessage packet);
}