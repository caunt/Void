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
using Void.Proxy.API.Network.IO.Streams.Packet;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins;
using Void.Proxy.API.Registries.Packets;
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

            holder.ClientboundRegistry = null;
            holder.ServerboundRegistry = null;
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

        holder.ClientboundRegistry = null;
        holder.ServerboundRegistry = null;
    }

    [Subscribe]
    public void OnSearchChannelBuilder(SearchChannelBuilderEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedHandshake(@event.Buffer))
            return;

        @event.Result = (direction, stream, builderCancellationToken) =>
        {
            var channel = new SimpleMinecraftChannel(new SimpleNetworkStream(stream));

            channel.Add(new MinecraftPacketMessageStream
            {
                Flow = direction,
                RegistryHolder = @event.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>()
            });

            return ValueTask.FromResult(channel as IMinecraftChannel);
        };
    }

    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case BufferedBinaryMessage bufferedBinaryMessage:
                logger.LogTrace("Received buffer length {Length} from {Side} {PlayerOrServer}", bufferedBinaryMessage.Memory.Length, @event.From, @event.From == Side.Client ? @event.Player : @event.Server);
                return;
            case BinaryPacket binaryPacket:
                logger.LogTrace("Received packet id {PacketId}, length {Length} from {Side} {PlayerOrServer}", binaryPacket.Id, binaryPacket.Memory.Length, @event.From, @event.From == Side.Client ? @event.Player : @event.Server);
                return;
            case HandshakePacket handshake:
                var holder = @event.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();
                holder.ClientboundRegistry = null;
                break;
        }

        logger.LogDebug("Received packet {Packet}", @event.Message);
    }

    [Subscribe]
    public void OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case BufferedBinaryMessage bufferedBinaryMessage:
                logger.LogTrace("Sent buffer length {Length} to {Direction} direction {PlayerOrServer}", bufferedBinaryMessage.Memory.Length, @event.To, @event.From == Side.Client ? @event.Player : @event.Server);
                return;
            case BinaryPacket binaryPacket:
                logger.LogTrace("Sent packet id {PacketId}, length {Length} to {Direction} direction {PlayerOrServer}", binaryPacket.Id, binaryPacket.Memory.Length, @event.To, @event.From == Side.Client ? @event.Player : @event.Server);
                return;
            case HandshakePacket handshake:
                var holder = @event.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();
                holder.ServerboundRegistry = null;
                break;
        }

        logger.LogDebug("Sent packet {Packet}", @event.Message);
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