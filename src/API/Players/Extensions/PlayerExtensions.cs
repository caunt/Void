using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Channels.Extensions;
using Void.Proxy.API.Network.IO.Channels.Services;
using Void.Proxy.API.Network.IO.Messages.Packets;
using Void.Proxy.API.Network.IO.Streams.Packet;
using Void.Proxy.API.Network.IO.Streams.Packet.Extensions;
using Void.Proxy.API.Network.IO.Streams.Packet.Registries;
using Void.Proxy.API.Plugins;
using Void.Proxy.API.Plugins.Services;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Players.Extensions;

public static class PlayerExtensions
{
    public static async ValueTask SendPacketAsync<T>(this IPlayer player, T packet, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        var channel = await player.GetChannelAsync(cancellationToken);
        await channel.SendPacketAsync(packet, cancellationToken);
    }

    public static async ValueTask RegisterPacketAsync<T>(this IPlayer player, CancellationToken cancellationToken = default, params MinecraftPacketMapping[] mappings) where T : IMinecraftPacket
    {
        var packetType = typeof(T);
        var plugins = player.Context.Services.GetRequiredService<IPluginService>();
        var plugin = plugins.All.FirstOrDefault(plugin => plugin.GetType().Assembly == packetType.Assembly)
            ?? throw new InvalidOperationException($"Plugin for packet {packetType.Name} not found.");

        var registry = await player.GetPluginPacketRegistryAsync(plugin, cancellationToken);
        registry.RegisterPacket<T>(player.ProtocolVersion, mappings);
    }

    public static async ValueTask ClearPluginsPacketRegistryAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var registries = await player.GetPluginsPacketRegistriesAsync(cancellationToken);

        foreach (var registry in registries.All)
            registry.Clear();
    }

    public static async ValueTask<IMinecraftPacketRegistry> GetPluginPacketRegistryAsync(this IPlayer player, IPlugin plugin, CancellationToken cancellationToken = default)
    {
        var registries = await player.GetPluginsPacketRegistriesAsync(cancellationToken);
        return registries.Get(plugin);
    }

    public static async ValueTask<IMinecraftPacketRegistryPlugins> GetPluginsPacketRegistriesAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var channel = await player.GetChannelAsync(cancellationToken);
        var stream = channel.Get<IMinecraftPacketMessageStream>();

        return stream.PluginsRegistryHolder ?? throw new Exception("Plugins registry holder is not set yet");
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