using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.States.Common;

public class HandshakeState : ProtocolState
{
    public Task<bool> HandleAsync(HandshakePacket packet)
    {
        var addressParts = packet.ServerAddress.Split('\0', StringSplitOptions.RemoveEmptyEntries);
        // var isForge = ForgeMarker.Range().Any(marker => addressParts.Contains(marker.Value));

        // if (isForge)
        //     link.Player.SetClientType(ClientType.Forge);
        // else if (addressParts.Length > 1)
        //     Console.WriteLine($"Player {link.Player} had extra marker(s) {string.Join(", ", addressParts[1..])} in handshake, ignoring");

        // link.SetProtocolVersion(ProtocolVersion.Get(packet.ProtocolVersion));
        // link.SwitchState(packet.NextState);
        // link.SaveHandshake(packet);

        return Task.FromResult(false);
    }
}