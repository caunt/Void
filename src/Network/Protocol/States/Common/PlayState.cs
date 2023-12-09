using MinecraftProxy.Network.Protocol.Packets.Clientbound;
using MinecraftProxy.Network.Protocol.Packets.Serverbound;
using MinecraftProxy.Network.Protocol.Packets.Shared;
using MinecraftProxy.Network.Protocol.Registry;
using MinecraftProxy.Network.Protocol.States.Custom;
using Nito.Disposables.Internals;
using System.Text;

namespace MinecraftProxy.Network.Protocol.States.Common;

public class PlayState(Player player, Server? server) : ProtocolState, ILoginConfigurePlayState, IConfigurePlayState
{
    protected override StateRegistry Registry { get; } = Registries.PlayStateRegistry;

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

    public Task<bool> HandleAsync(JoinGamePacket packet)
    {
        return Task.FromResult(false);
    }

    public Task<bool> HandleAsync(PluginMessage packet)
    {
        if (packet.Identifier == "minecraft:brand")
        {
            if (packet.Direction == Direction.Serverbound)
                player.SetBrand(Encoding.UTF8.GetString(packet.Data[1..]));
            else if (packet.Direction == Direction.Clientbound)
                server?.SetBrand(Encoding.UTF8.GetString(packet.Data[1..]));

            return Task.FromResult(false);
        }

        if (packet.Identifier == "minecraft:register")
        {
            var channels = Encoding.UTF8.GetString(packet.Data).Split('\0', StringSplitOptions.RemoveEmptyEntries);
            Proxy.Logger.Debug($"Received {packet.Direction} Play register channels message: {string.Join(", ", channels)}");

            return Task.FromResult(false);
        }

        if (packet.Identifier == "minecraft:unregister")
        {
            var channels = Encoding.UTF8.GetString(packet.Data).Split('\0', StringSplitOptions.RemoveEmptyEntries);
            Proxy.Logger.Debug($"Received {packet.Direction} Play unregister channels message: {string.Join(", ", channels)}");

            return Task.FromResult(false);
        }

        if (new[] { "minecraft", "forge", "fml" }.Any(name => packet.Identifier.Contains(name, StringComparison.InvariantCultureIgnoreCase)))
            Proxy.Logger.Debug($"Received {packet.Direction} Play plugin message in channel {packet.Identifier} with {packet.Data.Length} bytes");
        else
            Proxy.Logger.Verbose($"Received {packet.Direction} Play plugin message in channel {packet.Identifier} with {packet.Data.Length} bytes");

        return Task.FromResult(false);
    }

    public Task<bool> HandleAsync(ChatMessage packet)
    {
        Proxy.Logger.Information($"<{player}> {packet.Message}");
        return Task.FromResult(false);
    }
}