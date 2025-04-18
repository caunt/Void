using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Channels;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Network.Channels;
using Void.Proxy.Plugins.Common.Network.Channels.Services;
using Void.Proxy.Plugins.Common.Network.Streams.Network;
using Void.Proxy.Plugins.Common.Network.Streams.Packet;

namespace Void.Proxy.Plugins.Common.Services.Channels;

public abstract class AbstractChannelService(IEventService events) : IPluginCommonService
{
    [Subscribe]
    public static void OnProxyStarting(ProxyStartingEvent @event)
    {
        if (!@event.Services.HasService<IChannelBuilderService>())
            @event.Services.AddScoped<IChannelBuilderService, SimpleMinecraftChannelBuilderService>();
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

        await events.ThrowAsync(new ChannelCreatedEvent(player, side, channel), cancellationToken);

        return channel;
    }
}
