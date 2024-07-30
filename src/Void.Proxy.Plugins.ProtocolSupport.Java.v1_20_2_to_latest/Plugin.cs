using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Streams;
using Void.Proxy.API.Network.IO.Streams.Compression;
using Void.Proxy.API.Network.IO.Streams.Packet;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins;
using Void.Proxy.API.Registries.Packets;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest;

public class Plugin(ILogger<Plugin> logger, IPlayerService players) : IPlugin
{
    public static readonly ProtocolVersion[] SupportedVersions = ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.Latest);

    public string Name => nameof(Plugin);

    [Subscribe]
    public void OnProxyStarting(ProxyStartingEvent @event, CancellationToken cancellationToken)
    {
    }

    [Subscribe]
    public void OnProxyStopping(ProxyStoppingEvent @event, CancellationToken cancellationToken)
    {
    }

    [Subscribe]
    public void OnPluginLoad(PluginLoadEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Plugin != this)
            return;

        Mappings.Fill();
    }

    [Subscribe]
    public void OnPluginUnload(PluginUnloadEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Plugin != this)
            return;

        foreach (var player in players.All)
        {
            var holder = player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();

            if (holder.ManagedBy != this)
                continue;

            holder.Reset();
        }
    }

    [Subscribe]
    public void OnPlayerConnected(PlayerConnectedEvent @event, CancellationToken cancellationToken)
    {
        var holder = @event.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();

        if (!holder.IsEmpty)
            return;

        holder.ManagedBy = this;
        holder.ClientboundRegistry = new PacketRegistry { ProtocolVersion = @event.Player.ProtocolVersion, Mappings = Mappings.ClientboundHandshakeMappings };
        holder.ServerboundRegistry = new PacketRegistry { ProtocolVersion = @event.Player.ProtocolVersion, Mappings = Mappings.ServerboundHandshakeMappings };
    }

    [Subscribe]
    public void OnPlayerDisconnected(PlayerDisconnectedEvent @event, CancellationToken cancellationToken)
    {
        var holder = @event.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();

        if (holder.ManagedBy != this)
            return;

        holder.Reset();
    }

    [Subscribe]
    public void OnSearchChannelBuilder(SearchChannelBuilderEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedHandshake(@event.Buffer))
            return;

        @event.Result = (direction, stream, builderCancellationToken) =>
        {
            var channel = new SimpleMinecraftChannel(new SimpleNetworkStream(stream));
            channel.Add<MinecraftPacketMessageStream>();

            var packetStream = channel.Get<MinecraftPacketMessageStream>();
            packetStream.Flow = direction;
            packetStream.RegistryHolder = @event.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();

            return ValueTask.FromResult(channel as IMinecraftChannel);
        };
    }

    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case BufferedBinaryMessage bufferedBinaryMessage:
                logger.LogTrace("Received buffer length {Length} from {Side} {PlayerOrServer}", bufferedBinaryMessage.Holder.Slice.Length, @event.From, @event.From == Side.Client ? @event.Link.Player : @event.Link.Server);
                return;
            case BinaryPacket binaryPacket:
                logger.LogTrace("Received packet id {PacketId:X2}, length {Length} from {Side} {PlayerOrServer}", binaryPacket.Id, binaryPacket.Holder.Slice.Length, @event.From, @event.From == Side.Client ? @event.Link.Player : @event.Link.Server);
                return;
        }

        logger.LogDebug("Received packet {Packet}", @event.Message);

        switch (@event.Message)
        {
            case HandshakePacket handshake:
                @event.Link.Player.ProtocolVersion = ProtocolVersion.Get(handshake.ProtocolVersion);

                var holder = @event.Link.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();
                holder.ClientboundRegistry = new PacketRegistry { ProtocolVersion = @event.Link.Player.ProtocolVersion, Mappings = Mappings.ClientboundLoginMappings };
                break;
            case SetCompressionPacket setCompression:
                @event.Link.ServerChannel.AddBefore<MinecraftPacketMessageStream, ZlibCompressionMessageStream>();
                logger.LogDebug("Link {Link} enabled compression in server channel with threshold {CompressionThreshold}", @event.Link, setCompression.Threshold);

                var zlibStream = @event.Link.ServerChannel.Get<ZlibCompressionMessageStream>();
                zlibStream.CompressionThreshold = setCompression.Threshold;

                // cannot be awaited because default ILink implementation awaits for all event listeners one of which we are
                _ = @event.Link.RestartAsync(cancellationToken);
                break;
        }
    }

    [Subscribe]
    public void OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case BufferedBinaryMessage bufferedBinaryMessage:
                logger.LogTrace("Sent buffer length {Length} to {Direction} {PlayerOrServer}", bufferedBinaryMessage.Holder.Slice.Length, @event.To, @event.To == Side.Client ? @event.Link.Player : @event.Link.Server);
                return;
            case BinaryPacket binaryPacket:
                logger.LogTrace("Sent packet id {PacketId:X2}, length {Length} to {Direction} {PlayerOrServer}", binaryPacket.Id, binaryPacket.Holder.Slice.Length, @event.To, @event.To == Side.Client ? @event.Link.Player : @event.Link.Server);
                return;
        }

        logger.LogDebug("Sent packet {Packet}", @event.Message);
        
        switch (@event.Message)
        {
            case HandshakePacket handshake:
                var holder = @event.Link.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();
                holder.ServerboundRegistry = new PacketRegistry { ProtocolVersion = @event.Link.Player.ProtocolVersion, Mappings = Mappings.ServerboundLoginMappings };
                break;
            case SetCompressionPacket setCompression:
                @event.Link.PlayerChannel.AddBefore<MinecraftPacketMessageStream, ZlibCompressionMessageStream>();

                // ILink restart is already scheduled there, should we enable compression with client separately?

                var zlibStream = @event.Link.PlayerChannel.Get<ZlibCompressionMessageStream>();
                zlibStream.CompressionThreshold = setCompression.Threshold;

                logger.LogDebug("Link {Link} enabled compression in player channel with threshold {CompressionThreshold}", @event.Link, setCompression.Threshold);

                holder = @event.Link.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();
                holder.ServerboundRegistry = null;
                holder.ClientboundRegistry = null;
                break;
        }
    }

    public static bool IsSupportedHandshake(Memory<byte> memory)
    {
        try
        {
            var buffer = new MinecraftBuffer(memory.Span);
            var length = buffer.ReadVarInt();
            var packet = buffer.Read(length);

            buffer = new MinecraftBuffer(packet);
            var packetId = buffer.ReadVarInt();

            var decoded = HandshakePacket.Decode(ref buffer, ProtocolVersion.Oldest);
            return packetId == 0 && SupportedVersions.Contains(ProtocolVersion.Get(decoded.ProtocolVersion)) && !buffer.HasData;
        }
        catch
        {
            return false;
        }
    }
}