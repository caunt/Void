using Microsoft.Extensions.DependencyInjection;
using Void.Common.Network;
using Void.Common.Plugins;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Streams.Packet;
using Void.Minecraft.Network.Streams.Packet.Extensions;
using Void.Proxy.Api.Events.Chat;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Links.Extensions;
using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Network.IO.Channels.Extensions;
using Void.Proxy.Api.Network.IO.Channels.Services;
using Void.Proxy.Api.Plugins.Services;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Players.Extensions;

public static class PlayerExtensions
{
    public static async ValueTask<ChatMessageSendResult> SendChatMessageAsync(this IPlayer player, Component message, CancellationToken cancellationToken = default)
    {
        var events = player.Context.Services.GetRequiredService<IEventService>();
        return await events.ThrowWithResultAsync(new ChatMessageSendEvent(player, message), cancellationToken);
    }

    public static async ValueTask KickAsync(this IPlayer player, Component? reason = null, CancellationToken cancellationToken = default)
    {
        var players = player.Context.Services.GetRequiredService<IPlayerService>();
        await players.KickPlayerAsync(player, reason, cancellationToken);
    }

    public static async ValueTask SendPacketAsync<T>(this IPlayer player, T packet, CancellationToken cancellationToken = default) where T : IMinecraftPacket
    {
        var channel = await player.GetChannelAsync(cancellationToken);
        await channel.SendPacketAsync(packet, cancellationToken);
    }

    public static void RegisterPacket<T>(this IPlayer player, params MinecraftPacketIdMapping[] mappings) where T : IMinecraftPacket
    {
        var plugin = GetPacketPlugin<T>(player.Context.Services);
        var link = player.GetLink();
        var direction = typeof(T).IsAssignableTo(typeof(IMinecraftClientboundPacket)) ? Direction.Clientbound : Direction.Serverbound;
        var registry = link.GetPacketPluginsRegistries(direction).Get(plugin);

        registry.RegisterPacket<T>(player.ProtocolVersion, mappings);
    }

    public static void RegisterTransformations<T>(this IPlayer player, params MinecraftPacketTransformationMapping[] mappings) where T : IMinecraftPacket
    {
        var plugin = GetPacketPlugin<T>(player.Context.Services);
        var link = player.GetLink();
        var direction = typeof(T).IsAssignableTo(typeof(IMinecraftClientboundPacket)) ? Direction.Clientbound : Direction.Serverbound;
        var transformations = link.GetPacketPluginsTransformations(direction).Get(plugin);

        transformations.RegisterTransformations<T>(player.ProtocolVersion, mappings);
    }

    public static ILink GetLink(this IPlayer player)
    {
        var links = player.Context.Services.GetRequiredService<ILinkService>();

        if (!links.TryGetLink(player, out var link))
            throw new InvalidOperationException("Player is not linked to any server");

        return link;
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

    private static IPlugin GetPacketPlugin<T>(IServiceProvider services) where T : IMinecraftPacket
    {
        var packetType = typeof(T);
        var plugins = services.GetRequiredService<IPluginService>();
        var plugin = plugins.All.FirstOrDefault(plugin => plugin.GetType().Assembly == packetType.Assembly)
            ?? throw new InvalidOperationException($"Plugin for packet {packetType.Name} not found.");

        return plugin;
    }
}
