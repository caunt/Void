using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Channels;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Channels.Services;
using Void.Proxy.API.Players;
using Void.Proxy.Plugins.Common.Network.IO.Channels;
using Void.Proxy.Plugins.Common.Network.IO.Channels.Services;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Network;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Packet;
using Void.Proxy.Plugins.Common.Registries.Packets;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Common.Network.Protocol.Channels;

public abstract class AbstractChannelService(IEventService events) : IPluginService
{
    [Subscribe]
    public static void OnPlayerConnecting(PlayerConnectingEvent @event)
    {
        if (!@event.Services.HasService<IChannelBuilderService>())
            @event.Services.AddSingleton<IChannelBuilderService, SimpleChannelBuilderService>();
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

    private async ValueTask<IMinecraftChannel> ChannelBuilderAsync(IPlayer player, Direction direction, NetworkStream stream, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var channel = new SimpleChannel(new SimpleNetworkStream(stream));
        channel.Add<MinecraftPacketMessageStream>();

        var packetStream = channel.Get<MinecraftPacketMessageStream>();
        packetStream.RegistryHolder = new PacketRegistryHolder();

        await events.ThrowAsync(new ChannelCreatedEvent { Initiator = player, Channel = channel, Direction = direction }, cancellationToken);

        return channel;
    }
}