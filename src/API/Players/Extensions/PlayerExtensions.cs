using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Channels.Extensions;
using Void.Proxy.API.Network.IO.Channels.Services;
using Void.Proxy.API.Network.IO.Messages.Packets;
using Void.Proxy.API.Network.IO.Streams.Packet;
using Void.Proxy.API.Network.IO.Streams.Packet.Extensions;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Players.Extensions;

public static class PlayerExtensions
{
    public static async ValueTask SendPacketAsync<T>(this IPlayer player, T packet, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        var channel = await player.GetChannelAsync(cancellationToken);
        await channel.SendPacketAsync(packet, cancellationToken);
    }

    public static void RegisterPacket<T>(this IPlayer player, params MinecraftPacketMapping[] mappings) where T : IMinecraftPacket
    {
        player.GetPacketRegistry().RegisterPacket<T>(player.ProtocolVersion, mappings);
    }

    public static void ClearPacketRegistry(this IPlayer player)
    {
        player.GetPacketRegistry().Clear();
    }

    public static IMinecraftPacketRegistry GetPacketRegistry(this IPlayer player)
    {
        return player.Context.Services.GetRequiredService<IMinecraftPacketRegistry>();
    }

    public static async ValueTask<bool> IsProtocolSupportedAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
        return !channelBuilder.IsFallbackBuilder;
    }

    public static async ValueTask<IMinecraftChannel> BuildServerChannelAsync(this IPlayer player, IServer server, CancellationToken cancellationToken = default)
    {
        var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
        return await channelBuilder.BuildServerChannelAsync(player, server, cancellationToken);
    }

    public static async ValueTask<IMinecraftChannel> GetChannelAsync(this IPlayer player, CancellationToken cancellationToken = default)
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