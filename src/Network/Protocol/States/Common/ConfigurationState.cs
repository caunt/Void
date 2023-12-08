using MinecraftProxy.Network.Protocol.Packets.Clientbound;
using MinecraftProxy.Network.Protocol.Registry;
using MinecraftProxy.Network.Protocol.States.Custom;

namespace MinecraftProxy.Network.Protocol.States.Common;

public class ConfigurationState(Player player) : ProtocolState, IPlayableState
{
    protected override StateRegistry Registry { get; } = Registries.ConfigurationStateRegistry;

    public Task<bool> HandleAsync(DisconnectPacket packet)
    {
        return Task.FromResult(false);
    }

    public Task<bool> HandleAsync(FinishConfiguration finishConfiguration)
    {
        player.SwitchState(4);
        return Task.FromResult(false);
    }
}