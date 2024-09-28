using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Channels.Services;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Players;

public static class PlayerExtensions
{
    public static async ValueTask<IMinecraftChannel> BuildServerChannelAsync(this IPlayer player, IServer server, CancellationToken cancellationToken = default)
    {
        var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
        return await channelBuilder.BuildServerChannelAsync(server, cancellationToken);
    }

    public static async ValueTask<IMinecraftChannel> GetChannelAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        if (player.Context.Channel is not null)
            return player.Context.Channel;

        var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
        player.Context.Channel = await channelBuilder.BuildPlayerChannelAsync(player, cancellationToken);

        return player.Context.Channel;
    }

    private static async ValueTask<IMinecraftChannelBuilderService> GetChannelBuilderAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var channelBuilder = player.Context.Services.GetRequiredService<IMinecraftChannelBuilderService>();
        await channelBuilder.SearchChannelBuilderAsync(player, cancellationToken);

        return channelBuilder;
    }
}