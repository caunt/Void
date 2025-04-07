using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Void.Common.Network;
using Void.Common.Network.Channels;
using Void.Common.Players;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Channels;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Plugins.Common.Network.Channels;
using Void.Proxy.Plugins.Common.Network.Channels.Services;
using Void.Proxy.Plugins.Common.Network.Registries.PacketId;
using Void.Proxy.Plugins.Common.Network.Registries.Transformations;
using Void.Proxy.Plugins.Common.Network.Streams.Network;
using Void.Proxy.Plugins.Common.Network.Streams.Packet;

namespace Void.Proxy.Plugins.Common.Services.Channels;

public abstract class AbstractChannelService(IEventService events) : IPluginCommonService
{
    [Subscribe]
    public static void OnPlayerConnecting(PlayerConnectingEvent @event)
    {
        if (!@event.Services.HasService<IChannelBuilderService>())
            @event.Services.AddSingleton<IChannelBuilderService, SimpleMinecraftChannelBuilderService>();
    }

    [Subscribe]
    public void OnSearchChannelBuilder(SearchChannelBuilderEvent @event)
    {
        if (!@event.Player.TryGetMinecraftPlayer(out var player))
            return;

        if (!IsSupportedHandshake(@event.Buffer, out var protocolVersion))
            return;

        player.ProtocolVersion = protocolVersion;
        @event.Result = ChannelBuilderAsync;
    }

    protected abstract bool IsSupportedHandshake(Memory<byte> memory, [MaybeNullWhen(false)] out ProtocolVersion protocolVersion);

    private async ValueTask<INetworkChannel> ChannelBuilderAsync(IPlayer player, Side side, NetworkStream stream, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var channel = new SimpleMinecraftChannel(new SimpleNetworkStream(stream));
        channel.Add<MinecraftPacketMessageStream>();

        var packetStream = channel.Get<MinecraftPacketMessageStream>();
        packetStream.SystemRegistryHolder = new MinecraftPacketIdSystemRegistry();
        packetStream.PluginsRegistryHolder = new MinecraftPacketIdPluginsRegistry();
        packetStream.TransformationsHolder = new MinecraftPacketPluginsTransformations();

        await events.ThrowAsync(new ChannelCreatedEvent(player, side, channel), cancellationToken);

        return channel;
    }
}
