using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Channels;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Channels.Services;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Players;
using Void.Proxy.Plugins.Common.Network.IO.Channels;
using Void.Proxy.Plugins.Common.Network.IO.Channels.Services;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Network;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Packet;
using Void.Proxy.Plugins.Common.Registries.Packets;
using Void.Proxy.Plugins.Common.Services;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Services;

public class ChannelService(IEventService events) : IPluginService
{
    [Subscribe]
    public void OnPlayerConnecting(PlayerConnectingEvent @event, CancellationToken cancellationToken)
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

    private static bool IsSupportedHandshake(Memory<byte> memory, [MaybeNullWhen(false)] out ProtocolVersion protocolVersion)
    {
        try
        {
            var buffer = new MinecraftBuffer(memory.Span);
            var length = buffer.ReadVarInt();
            var packet = buffer.Read(length);

            buffer = new MinecraftBuffer(packet);
            var packetId = buffer.ReadVarInt();

            var decoded = HandshakePacket.Decode(ref buffer, Plugin.SupportedVersions.First());
            protocolVersion = ProtocolVersion.Get(decoded.ProtocolVersion);

            return packetId == 0 && Plugin.SupportedVersions.Contains(protocolVersion) && !buffer.HasData;
        }
        catch
        {
            protocolVersion = null;
            return false;
        }
    }
}