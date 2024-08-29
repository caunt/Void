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
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.API.Network.IO.Streams;
using Void.Proxy.API.Network.IO.Streams.Compression;
using Void.Proxy.API.Network.IO.Streams.Encryption;
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

                @event.Link.Player.ProtocolVersion = playerProtocolVersion;

                var holder = @event.Link.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();
                holder.ClientboundRegistry = new PacketRegistry { ProtocolVersion = @event.Link.Player.ProtocolVersion, Mappings = Mappings.ClientboundLoginMappings };
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
                holder = @event.Link.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();
                holder.ClientboundRegistry = new PacketRegistry { ProtocolVersion = @event.Link.Player.ProtocolVersion, Mappings = Mappings.ClientboundConfigurationMappings };
                break;
            case FinishConfigurationPacket finishConfiguration:
                holder = @event.Link.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();
                holder.ServerboundRegistry = new PacketRegistry { ProtocolVersion = @event.Link.Player.ProtocolVersion, Mappings = Mappings.ServerboundPlayMappings };
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
                var holder = @event.Link.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();
                holder.ServerboundRegistry = new PacketRegistry { ProtocolVersion = @event.Link.Player.ProtocolVersion, Mappings = Mappings.ServerboundLoginMappings };
                break;
            case SetCompressionPacket setCompression:
                @event.Link.PlayerChannel.AddBefore<MinecraftPacketMessageStream, SharpZipLibCompressionMessageStream>();

                // ILink restart is already scheduled here, should we enable compression with client separately?

                var zlibStream = @event.Link.PlayerChannel.Get<SharpZipLibCompressionMessageStream>();
                zlibStream.CompressionThreshold = setCompression.Threshold;

                logger.LogDebug("Link {Link} enabled compression in player channel with threshold {CompressionThreshold}", @event.Link, setCompression.Threshold);
                break;
            case LoginAcknowledgedPacket loginAcknowledged:
                holder = @event.Link.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();
                holder.ServerboundRegistry = new PacketRegistry { ProtocolVersion = @event.Link.Player.ProtocolVersion, Mappings = Mappings.ServerboundConfigurationMappings };
                break;
            case FinishConfigurationPacket finishConfiguration:
                holder = @event.Link.Player.Scope.ServiceProvider.GetRequiredService<IPacketRegistryHolder>();
                holder.ClientboundRegistry = new PacketRegistry { ProtocolVersion = @event.Link.Player.ProtocolVersion, Mappings = Mappings.ClientboundPlayMappings };
                break;
            case EncryptionRequestPacket encryptionRequest:
                logger.LogDebug(JsonSerializer.Serialize(encryptionRequest));
                break;
            case EncryptionResponsePacket encryptionResponse:
                var serverPrivateKey = Convert.FromHexString("30820276020100300d06092a864886f70d0101010500048202603082025c0201000281810084e9eaab73f9a5eb70b8388b275bb0c9f3ff0d9c0bf86970161fb8e4d6cfa4018df8c40ae67001e7d26f8b855dcaf8eb202636dbe9e19a5b68845985aef317cacfb7e23593ea2eb98454e0c0baa466c2b11dc1d1744c38f023755b84dd1a273b2a5c3f09aaafffea1c2c7d145ef8622d0dd1bc8359b992a18ee1cee2528eb77f020301000102818008cd025c6af7ff5c092131a149306192e5c4a02e927e56e0f49e121c98fab3c5e49431caf4fa3aae12798f57fbdf6a3f0b686c5e806c8f4f792ab650cb464e6f669ac54d2131d8c777d7080b54155c6a4a83b3970d1a8cc8079ff0dbed58c536a3594f9cb2db63fafdaf9df0fcc87af51aee297c711135eacebca152167d67410241008c4040c6e1cbb1d98f7f7a7a3bef5523a472ebfca12fd0d0901fca24564e6fd58df66eb499ae26e7992600e519964185da1ad2c1ed1952da375e9c67986948bf024100f29b7970e88312857bcf3dd80d758892573493e44c83357194cf90f8ce04a41c92eeae06fdb614b02d6402ccea9fe1062ff3e324695039e0d8e3438b8595814102410088b35feff9c956d25da1bd39430de6085593861cb8e7283b011f5b21cbd5abff94dd6bce3034a4cafc65245e297060f11c4324c5cc59f07dad9654104d67e17502402a5866cf0556737227151a374eca18076aff3b5d1ad9c0074e31189dc4dfdc813c483ac9ef98cb6da0ce970a8b5d529a90de21e4661961b0d44a7eaca8a95ac102403773371ef2776d2e87d8b1a2e4814f3e44f4961f909926b056361dc843dfdb31d3fafd3bd0d3c2956c0fd386cb68ba55464a439feec4f9be60530b8f76c6bdca");

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