using MinecraftProxy.Network.Protocol.Packets.Clientbound;

namespace MinecraftProxy.Network.Protocol.States;

public class ConfigurationState(Player player) : ProtocolState, IPlayableState
{
    protected override Dictionary<int, Type> serverboundPackets => new()
    {
    };

    protected override Dictionary<int, Type> clientboundPackets => new()
    {
        { 0x01, typeof(DisconnectPacket) },
        { 0x02, typeof(FinishConfiguration) }
    };

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