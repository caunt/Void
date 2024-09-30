using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Channels;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Channels.Services;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Channels;
using Void.Proxy.Common.Network.IO.Channels.Services;
using Void.Proxy.Common.Network.IO.Streams.Network;
using Void.Proxy.Common.Network.IO.Streams.Packet;
using Void.Proxy.Common.Registries.Packets;
using Void.Proxy.Common.Services;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Services;

public class ChannelService : IPluginService
{
    [Subscribe]
    public void OnPlayerConnecting(PlayerConnectingEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Services.HasService<IMinecraftChannelBuilderService>())
            @event.Services.AddSingleton<IMinecraftChannelBuilderService, SimpleChannelBuilderService>();
    }

    [Subscribe]
    public void OnSearchChannelBuilder(SearchChannelBuilderEvent @event)
    {
        if (!IsSupportedHandshake(@event.Buffer, out var protocolVersion))
            return;

        var registries = @event.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();

        @event.Player.ProtocolVersion = protocolVersion;
        @event.Result = (direction, networkStream, cancellationToken) => ChannelBuilderAsync(registries, direction, networkStream, cancellationToken);
    }

    private static ValueTask<IMinecraftChannel> ChannelBuilderAsync(IPacketRegistryHolder registries, Direction direction, NetworkStream stream, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var channel = new SimpleChannel(new SimpleNetworkStream(stream));
        channel.Add<MinecraftPacketMessageStream>();

        var packetStream = channel.Get<MinecraftPacketMessageStream>();
        packetStream.Flow = direction;
        packetStream.RegistryHolder = registries;

        return ValueTask.FromResult(channel as IMinecraftChannel);
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