using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Channels.Services;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins;
using Void.Proxy.Common.Network.IO.Channels;
using Void.Proxy.Common.Network.IO.Channels.Services;
using Void.Proxy.Common.Network.IO.Messages.Binary;
using Void.Proxy.Common.Network.IO.Streams.Compression;
using Void.Proxy.Common.Network.IO.Streams.Encryption;
using Void.Proxy.Common.Network.IO.Streams.Network;
using Void.Proxy.Common.Network.IO.Streams.Packet;
using Void.Proxy.Common.Registries.Packets;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest;

public class Plugin(ILogger<Plugin> logger, IPlayerService players) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.Latest);

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
            var holder = player.Context.Services.GetRequiredService<IPacketRegistryHolder>();

            if (holder.ManagedBy != this)
                continue;

            holder.Reset();
        }
    }

    [Subscribe]
    public void OnPlayerConnecting(PlayerConnectingEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Services.HasService<IMinecraftChannelBuilderService>())
            @event.Services.AddSingleton<IMinecraftChannelBuilderService, SimpleChannelBuilderService>();

        if (!@event.Services.HasService<IPacketRegistryHolder>())
            @event.Services.AddSingleton<IPacketRegistryHolder, PacketRegistryHolder>();
    }

    [Subscribe]
    public void OnPlayerConnected(PlayerConnectedEvent @event, CancellationToken cancellationToken)
    {
        if (!SupportedVersions.Contains(@event.Player.ProtocolVersion))
            return;

        var holder = @event.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();

        if (!holder.IsEmpty)
            return;

        holder.ManagedBy = this;
        holder.ProtocolVersion = @event.Player.ProtocolVersion;

        holder.ReplacePackets(Direction.Clientbound, Mappings.ClientboundHandshakeMappings);
        holder.ReplacePackets(Direction.Serverbound, Mappings.ServerboundHandshakeMappings);
    }

    [Subscribe]
    public void OnPlayerDisconnected(PlayerDisconnectedEvent @event, CancellationToken cancellationToken)
    {
        var holder = @event.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();

        if (holder.ManagedBy != this)
            return;

        holder.Reset();
    }

    [Subscribe]
    public void OnSearchChannelBuilder(SearchChannelBuilderEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedHandshake(@event.Buffer, out var protocolVersion))
            return;

        @event.Player.ProtocolVersion = protocolVersion;
        @event.Result = (direction, stream, builderCancellationToken) =>
        {
            var channel = new SimpleChannel(new SimpleNetworkStream(stream));
            channel.Add<MinecraftPacketMessageStream>();

            var packetStream = channel.Get<MinecraftPacketMessageStream>();
            packetStream.Flow = direction;
            packetStream.RegistryHolder = @event.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();

            return ValueTask.FromResult(channel as IMinecraftChannel);
        };
    }

    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case BufferedBinaryMessage bufferedBinaryMessage:
                logger.LogTrace("Received buffer length {Length} from {Side} {PlayerOrServer}", bufferedBinaryMessage.Stream.Length, @event.From, @event.From == Side.Client ? @event.Link.Player : @event.Link.Server);
                return;
            case BinaryPacket binaryPacket:
                logger.LogTrace("Received packet id {PacketId:X2}, length {Length} from {Side} {PlayerOrServer}", binaryPacket.Id, binaryPacket.Stream.Length, @event.From, @event.From == Side.Client ? @event.Link.Player : @event.Link.Server);
                return;
        }

        logger.LogDebug("Received packet {Packet}", @event.Message);

        switch (@event.Message)
        {
            case HandshakePacket handshake:
                var playerProtocolVersion = ProtocolVersion.Get(handshake.ProtocolVersion);

                if (!SupportedVersions.Contains(playerProtocolVersion))
                    break;

                var holder = @event.Link.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();
                holder.ReplacePackets(Direction.Clientbound, Mappings.ClientboundLoginMappings);
                break;
            case SetCompressionPacket setCompression:
                @event.Link.ServerChannel.AddBefore<MinecraftPacketMessageStream, SharpZipLibCompressionMessageStream>();
                logger.LogDebug("Link {Link} enabled compression in server channel with threshold {CompressionThreshold}", @event.Link, setCompression.Threshold);

                var zlibStream = @event.Link.ServerChannel.Get<SharpZipLibCompressionMessageStream>();
                zlibStream.CompressionThreshold = setCompression.Threshold;

                // cannot be awaited because default ILink implementation awaits for all event listeners one of which we are
                var restartTask = @event.Link.RestartAsync(cancellationToken);
                break;
            case LoginAcknowledgedPacket loginAcknowledged:
                holder = @event.Link.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();
                holder.ReplacePackets(Direction.Clientbound, Mappings.ClientboundConfigurationMappings);
                break;
            case FinishConfigurationPacket finishConfiguration:
                holder = @event.Link.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();
                holder.ReplacePackets(Direction.Serverbound, Mappings.ServerboundPlayMappings);
                break;
            case KeepAliveResponsePacket keepAliveResponse:
                logger.LogDebug("Player {Link} sent keep alive response ({Id})", @event.Link.Player, keepAliveResponse.Id);
                break;
        }
    }

    [Subscribe]
    public void OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case BufferedBinaryMessage bufferedBinaryMessage:
                logger.LogTrace("Sent buffer length {Length} to {Direction} {PlayerOrServer}", bufferedBinaryMessage.Stream.Length, @event.To, @event.To == Side.Client ? @event.Link.Player : @event.Link.Server);
                return;
            case BinaryPacket binaryPacket:
                logger.LogTrace("Sent packet id {PacketId:X2}, length {Length} to {Direction} {PlayerOrServer}", binaryPacket.Id, binaryPacket.Stream.Length, @event.To, @event.To == Side.Client ? @event.Link.Player : @event.Link.Server);
                return;
        }

        logger.LogDebug("Sent packet {Packet}", @event.Message);

        switch (@event.Message)
        {
            case HandshakePacket handshake:
                var holder = @event.Link.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();
                holder.ReplacePackets(Direction.Serverbound, Mappings.ServerboundLoginMappings);
                break;
            case SetCompressionPacket setCompression:
                @event.Link.PlayerChannel.AddBefore<MinecraftPacketMessageStream, SharpZipLibCompressionMessageStream>();

                // ILink restart is already scheduled here, should we enable compression with client separately?

                var zlibStream = @event.Link.PlayerChannel.Get<SharpZipLibCompressionMessageStream>();
                zlibStream.CompressionThreshold = setCompression.Threshold;

                logger.LogDebug("Link {Link} enabled compression in player channel with threshold {CompressionThreshold}", @event.Link, setCompression.Threshold);
                break;
            case LoginAcknowledgedPacket loginAcknowledged:
                holder = @event.Link.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();
                holder.ReplacePackets(Direction.Serverbound, Mappings.ServerboundConfigurationMappings);
                break;
            case FinishConfigurationPacket finishConfiguration:
                holder = @event.Link.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();
                holder.ReplacePackets(Direction.Clientbound, Mappings.ClientboundPlayMappings);
                break;
            case EncryptionRequestPacket encryptionRequest:
                logger.LogDebug(JsonSerializer.Serialize(encryptionRequest));
                break;
            case EncryptionResponsePacket encryptionResponse:
                var serverPrivateKey = Convert.FromHexString("30820278020100300d06092a864886f70d0101010500048202623082025e0201000281810080175813972cd94d469447872ae600b6a69be8f82848d9de1e71f842732b321e308ab66998e73361bfcd3a8c48cf655462260142d2e41385d5667a92acb18ba67c88b916188ca02d1f808747cd17dadece86e4c778308f70ce309b374aff842c2e8d3d25df0531a7f62da8ff5a874d53f264ee5086b80907ca435a69705ad6b90203010001028180019e910bb8233b92bdc15bb696fc2451802c2e6ff2118c6241bc80b0002886a5926589382efece2f6212d60c19fd545e12a1cfb4b6e7d7f0ad1b72ae60428dedf7bed8055d2ee3831d1d07d50b328d18f0d8e006153b2ba865d4750c61df26f380f4d80676f95b18f2bbfcce0a71b6051c16ea4586e81064ad23e8ecbac6e555024100cba03a4a47a8e83e2d707eb49d8c7ec945c87ff31be481f26a11c131d92ab4e9ab29369cf51f9d1f24bf9e3692647e4c53cf528a25b11850ce7ed7cccf0ebb47024100a10986401040ea803e55800e25adf12a522e6b318e8bf9b731b3320f1dba9a0d288c78dca5279334622f882f023b0a2ad7bc87c2db4cd8cc57f80dd43beeddff024100890f8bcccdd95aa1e7920e2762d8dbca7cc74da08508d5932764560748a71f691d85bb360124cb6cd81e86cf32d0a3d69a9f356eb99a2fc4cc89c296205549270241008c11e8bfd8635e9565a2a0dbad527aae4105371fc7c960cf435f866f37809376568ab8a5d2d1756cdeea511df266c0153bc9349cfc7aff27de6c583afe566aa9024100af9a133d4693e0fbd3f163115d06b92f512670bb29373149a9b63ced40086f6d5b66201fe1e2a071c9214af3aa63f9d90902d3c2afbc0d1b811d8f8426daec3d");

                var secret = Decrypt(serverPrivateKey, encryptionResponse.SharedSecret);

                @event.Link.ServerChannel.AddBefore<MinecraftPacketMessageStream, AesCfb8BufferedStream>(new AesCfb8BufferedStream(secret));
                @event.Link.PlayerChannel.AddBefore<MinecraftPacketMessageStream, AesCfb8BufferedStream>(new AesCfb8BufferedStream(secret));

                // cannot be awaited because default ILink implementation awaits for all event listeners one of which we are
                var restartTask = @event.Link.RestartAsync(cancellationToken);
                break;

                static byte[] Decrypt(byte[] key, byte[] data)
                {
                    using var rsa = RSA.Create();
                    rsa.ImportPkcs8PrivateKey(key, out _);
                    return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
                }
        }
    }

    public static bool IsSupportedHandshake(Memory<byte> memory, [MaybeNullWhen(false)] out ProtocolVersion protocolVersion)
    {
        try
        {
            var buffer = new MinecraftBuffer(memory.Span);
            var length = buffer.ReadVarInt();
            var packet = buffer.Read(length);

            buffer = new MinecraftBuffer(packet);
            var packetId = buffer.ReadVarInt();

            var decoded = HandshakePacket.Decode(ref buffer, SupportedVersions.First());
            protocolVersion = ProtocolVersion.Get(decoded.ProtocolVersion);

            return packetId == 0 && SupportedVersions.Contains(protocolVersion) && !buffer.HasData;
        }
        catch
        {
            protocolVersion = null;
            return false;
        }
    }
}