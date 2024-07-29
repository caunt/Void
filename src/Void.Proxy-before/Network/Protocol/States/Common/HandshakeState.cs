using Void.Proxy.Models.General;
using Void.Proxy.Network.Protocol.Forge;
using Void.Proxy.Network.Protocol.Packets.Serverbound;
using Void.Proxy.Network.Protocol.Registry;

namespace Void.Proxy.Network.Protocol.States.Common;

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
        link.SaveHandshake(packet);

        return Task.FromResult(false);
    }
}