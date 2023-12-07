using MinecraftProxy.Network.Protocol.Packets.Clientbound;
using MinecraftProxy.Network.Protocol.Packets.Serverbound;
using MinecraftProxy.Network.Protocol.States.Custom;
using Nito.Disposables.Internals;

namespace MinecraftProxy.Network.Protocol.States.Common;

public class PlayState(Player player) : ProtocolState, IPlayableState
{
    protected override Dictionary<int, Type> serverboundPackets => new()
    {
        { 0x06, typeof(PlayerSessionPacket) }
    };

    protected override Dictionary<int, Type> clientboundPackets => new()
    {
        { 0x1B, typeof(DisconnectPacket) },
        { 0x3C, typeof(PlayerInfoUpdatePacket) }
    };

    public Task<bool> HandleAsync(DisconnectPacket packet)
    {
        return Task.FromResult(false);
    }

    public Task<bool> HandleAsync(PlayerSessionPacket packet)
    {
        player.SetIdentifiedKey(packet.IdentifiedKey);
        return Task.FromResult(true); // should not cancel?
    }

    public Task<bool> HandleAsync(PlayerInfoUpdatePacket packet)
    {
        // does this even needed?
        foreach (var playerInfo in packet.Players)
        {
            var addPlayerAction = playerInfo.Actions.Select(action => action as PlayerInfoUpdatePacket.AddPlayerAction).WhereNotNull().FirstOrDefault();

            if (addPlayerAction is null)
                continue;

            if (player.GameProfile is null)
                continue;

            if (playerInfo.Guid == player.GameProfile.Id)
            {
                addPlayerAction.Name = player.GameProfile.Name;
                addPlayerAction.Properties = player.GameProfile.Properties;
            }

            // add other players
        }

        return Task.FromResult(false);
    }
}