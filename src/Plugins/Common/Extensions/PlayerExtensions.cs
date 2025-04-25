using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Events;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.Transformations.Extensions;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Players;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.Common.Players.Contexts;

namespace Void.Proxy.Plugins.Common.Extensions;

public static class PlayerExtensions
{
    public static void RegisterSystemTransformations<T>(this IMinecraftPlayer player, params MinecraftPacketTransformationMapping[] mappings) where T : IMinecraftPacket
    {
        var link = player.GetLink();
        link.GetRegistries(Direction.Clientbound).PacketTransformationsSystem.All.RegisterTransformations<T>(player.ProtocolVersion, mappings);
        link.GetRegistries(Direction.Serverbound).PacketTransformationsSystem.All.RegisterTransformations<T>(player.ProtocolVersion, mappings);
    }

    public static async ValueTask SetPhaseAsync(this IMinecraftPlayer player, Side side, Phase phase, INetworkChannel channel, CancellationToken cancellationToken)
    {
        if (side is Side.Client)
            player.Phase = phase;

        var events = player.Context.Services.GetRequiredService<IEventService>();
        await events.ThrowAsync(new PhaseChangedEvent(player, side, channel, phase), cancellationToken);
    }

    public static async ValueTask<IMinecraftPlayer> UpgradeToMinecraftAsync(this IPlayer player, ProtocolVersion protocolVersion, CancellationToken cancellationToken)
    {
        var minecraftPlayer = new MinecraftPlayer(player.Client, player.Context, player.RemoteEndPoint, protocolVersion);

        if (player.Context is PlayerContext playerContext)
            playerContext.Player = minecraftPlayer;

        var players = player.Context.Services.GetRequiredService<IPlayerService>();
        await players.UpgradeAsync(player, minecraftPlayer, cancellationToken);

        return minecraftPlayer;
    }
}
