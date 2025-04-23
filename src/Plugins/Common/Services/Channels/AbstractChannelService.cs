using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Channels;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.Channels;
using Void.Proxy.Plugins.Common.Network.Streams.Network;
using Void.Proxy.Plugins.Common.Network.Streams.Packet;

namespace Void.Proxy.Plugins.Common.Services.Channels;

public abstract class AbstractChannelService(IEventService events) : IPluginCommonService
{
    [Subscribe]
    public async ValueTask OnSearchChannelBuilder(SearchChannelBuilderEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedHandshake(@event.Buffer, out var protocolVersion))
            return;

        // This is definitely a Minecraft connection
        var player = await @event.Player.UpgradeToMinecraftAsync(protocolVersion, cancellationToken);

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
