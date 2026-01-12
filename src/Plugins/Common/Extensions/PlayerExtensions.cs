using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.Common.Players.Contexts;

namespace Void.Proxy.Plugins.Common.Extensions;

public static class PlayerExtensions
{
    public static async ValueTask SetPhaseAsync(this IPlayer player, ILink? link, Side side, Phase phase, INetworkChannel channel, CancellationToken cancellationToken)
    {
        player.Logger.LogTrace("Setting {Side} phase to {Phase}", side, phase);

        if (side is Side.Client)
            player.Phase = phase;

        var events = player.Context.Services.GetRequiredService<IEventService>();
        await events.ThrowAsync(new PhaseChangedEvent(link, player, side, channel, phase), cancellationToken);
    }

    public static async ValueTask<IPlayer> UpgradeToMinecraftAsync(this IPlayer player, ProtocolVersion protocolVersion, CancellationToken cancellationToken)
    {
        var minecraftPlayer = new MinecraftPlayer(player.Client, player.Context, player.RemoteEndPoint, player.ConnectedAt, protocolVersion);

        if (player.Context is PlayerContext playerContext)
            playerContext.Player = minecraftPlayer;

        var players = player.Context.Services.GetRequiredService<IPlayerService>();
        await players.UpgradeAsync(player, minecraftPlayer, cancellationToken);

        return minecraftPlayer;
    }
}
