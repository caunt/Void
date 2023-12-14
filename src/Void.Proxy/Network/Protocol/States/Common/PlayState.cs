using Nito.Disposables.Internals;
using System.Text;
using Void.Proxy.Commands;
using Void.Proxy.Models.General;
using Void.Proxy.Network.Protocol.Packets.Clientbound;
using Void.Proxy.Network.Protocol.Packets.Serverbound;
using Void.Proxy.Network.Protocol.Packets.Shared;
using Void.Proxy.Network.Protocol.Registry;
using Void.Proxy.Network.Protocol.States.Custom;

namespace Void.Proxy.Network.Protocol.States.Common;

public class PlayState(Link link) : ProtocolState, ILoginConfigurePlayState, IConfigurePlayState
{
    protected override StateRegistry Registry { get; } = Registries.PlayStateRegistry;

    public Task<bool> HandleAsync(DisconnectPacket packet)
    {
        return Task.FromResult(false);
    }

    public Task<bool> HandleAsync(PlayerSessionPacket packet)
    {
        link.Player.SetIdentifiedKey(packet.IdentifiedKey);
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

            if (link.Player.GameProfile is null)
                continue;

            if (playerInfo.Guid == link.Player.GameProfile.Id)
            {
                addPlayerAction.Name = link.Player.GameProfile.Name;
                addPlayerAction.Properties = link.Player.GameProfile.Properties;
            }

            // add other players
        }

        return Task.FromResult(false);
    }

    public async Task<bool> HandleAsync(JoinGamePacket packet)
    {
        await link.Player.SendPacketAsync(packet);

        if (link.IsSwitching)
        {
            await link.Player.SendPacketAsync(RespawnPacket.FromJoinGame(packet));
            link.SwitchComplete();
        }

        return true;
    }

    public Task<bool> HandleAsync(PluginMessage packet)
    {
        if (packet.Identifier == "minecraft:brand")
        {
            if (packet.Direction == Direction.Serverbound)
                link.Player.SetBrand(Encoding.UTF8.GetString(packet.Data[1..]));
            else if (packet.Direction == Direction.Clientbound)
                link.Server.SetBrand(Encoding.UTF8.GetString(packet.Data[1..]));

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

    public Task<bool> HandleAsync(IChatMessage packet)
    {
        Proxy.Logger.Information($"<{link.Player}> {packet.Message}");
        return Task.FromResult(false);
    }

    public async Task<bool> HandleAsync(IChatCommand packet)
    {
        Proxy.Logger.Information($"{link.Player} issued server command /{packet.Command}");
        return await CommandExecutor.ExecuteAsync(link, packet.Command);
    }

    public Task<bool> HandleAsync(SystemChatMessage packet)
    {
        return Task.FromResult(false);
    }

    public Task<bool> HandleAsync(RespawnPacket packet)
    {
        return Task.FromResult(false);
    }
}