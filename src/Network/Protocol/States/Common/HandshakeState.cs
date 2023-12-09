using MinecraftProxy.Models.General;
using MinecraftProxy.Network.Protocol.Forge;
using MinecraftProxy.Network.Protocol.Packets.Serverbound;
using MinecraftProxy.Network.Protocol.Registry;

namespace MinecraftProxy.Network.Protocol.States.Common;

public class HandshakeState(Link link) : ProtocolState
{
    protected override StateRegistry Registry { get; } = Registries.HandshakeStateRegistry;

    public Task<bool> HandleAsync(HandshakePacket packet)
    {
        var addressParts = packet.ServerAddress.Split('\0', StringSplitOptions.RemoveEmptyEntries);
        var isForge = ForgeMarker.Range().Any(marker => addressParts.Contains(marker.Value));

        if (isForge)
            link.Player.SetClientType(ClientType.Forge);
        else if (addressParts.Length > 1)
            Proxy.Logger.Warning($"Player {link.Player} had extra marker(s) {string.Join(", ", addressParts[1..])} in handshake, ignoring");

        link.SetProtocolVersion(ProtocolVersion.Get(packet.ProtocolVersion));
        link.SwitchState(packet.NextState);

        return Task.FromResult(false);
    }
}