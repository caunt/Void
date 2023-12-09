using MinecraftProxy.Network.Protocol.Packets.Shared;

namespace MinecraftProxy.Network.Protocol.States.Custom;

public interface IConfigurePlayState : IProtocolState
{
    public Task<bool> HandleAsync(PluginMessage packet);
}