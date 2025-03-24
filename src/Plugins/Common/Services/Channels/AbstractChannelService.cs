using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Channels;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Network.IO.Channels.Services;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Network.IO.Channels;
using Void.Proxy.Plugins.Common.Network.IO.Channels.Services;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Network;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Packet;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Registries;

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
        if (!IsSupportedHandshake(@event.Buffer, out var protocolVersion))
            return;

        @event.Player.ProtocolVersion = protocolVersion;
        @event.Result = ChannelBuilderAsync;
    }

    protected abstract bool IsSupportedHandshake(Memory<byte> memory, [MaybeNullWhen(false)] out ProtocolVersion protocolVersion);

    private async ValueTask<IMinecraftChannel> ChannelBuilderAsync(IPlayer player, Side side, NetworkStream stream, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var channel = new SimpleMinecraftChannel(new SimpleNetworkStream(stream));
        channel.Add<MinecraftPacketMessageStream>();

        var packetStream = channel.Get<MinecraftPacketMessageStream>();
        packetStream.SystemRegistryHolder = new MinecraftPacketRegistrySystem();
        packetStream.PluginsRegistryHolder = new MinecraftPacketRegistryPlugins();

        await events.ThrowAsync(new ChannelCreatedEvent(player, side, channel), cancellationToken);

        return channel;
    }
}