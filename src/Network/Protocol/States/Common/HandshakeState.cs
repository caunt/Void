using MinecraftProxy.Network.Protocol.Packets.Serverbound;
using MinecraftProxy.Network.Protocol.Registry;

namespace MinecraftProxy.Network.Protocol.States.Common;

public class HandshakeState(Player player, Server? server) : ProtocolState
{
    protected override StateRegistry Registry { get; } = Registries.HandshakeStateRegistry;

    public Task<bool> HandleAsync(HandshakePacket packet)
    {
        player.SetProtocolVersion(ProtocolVersion.Get(packet.ProtocolVersion));
        player.SwitchState(packet.NextState);
        return Task.FromResult(false);
    }
}