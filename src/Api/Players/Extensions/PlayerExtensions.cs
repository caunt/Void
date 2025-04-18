using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Void.Common.Network.Channels;
using Void.Common.Players;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Players.Extensions;

public static class PlayerExtensions
{
    public static IServer? GetServer(this IPlayer player)
    {
        if (!player.TryGetLink(out var link))
            return null;

        return link.Server;
    }

    public static bool TryGetLink(this IPlayer player, [MaybeNullWhen(false)] out ILink link)
    {
        var links = player.Context.Services.GetRequiredService<ILinkService>();
        return links.TryGetLink(player, out link);
    }

    public static ILink GetLink(this IPlayer player)
    {
        if (!player.TryGetLink(out var link))
            throw new InvalidOperationException("Player is not linked to any server");

        return link;
    }

    public static async ValueTask KickAsync(this IPlayer player, string text, CancellationToken cancellationToken = default)
    {
        var players = player.Context.Services.GetRequiredService<IPlayerService>();
        await players.KickPlayerAsync(player, text, cancellationToken);
    }

    public static async ValueTask<bool> IsProtocolSupportedAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
        return !channelBuilder.IsFallbackBuilder;
    }

    public static async ValueTask<INetworkChannel> BuildServerChannelAsync(this IPlayer player, IServer server, CancellationToken cancellationToken = default)
    {
        var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
        return await channelBuilder.BuildServerChannelAsync(player, server, cancellationToken);
    }

    public static async ValueTask<INetworkChannel> GetChannelAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        if (player.Context.Channel is not null)
            return player.Context.Channel;

        var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
        player.Context.Channel = await channelBuilder.BuildPlayerChannelAsync(player, cancellationToken);

        return player.Context.Channel;
    }

    private static async ValueTask<IChannelBuilderService> GetChannelBuilderAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var channelBuilder = player.Context.Services.GetRequiredService<IChannelBuilderService>();
        await channelBuilder.SearchChannelBuilderAsync(player, cancellationToken);

        return channelBuilder;
    }
}
