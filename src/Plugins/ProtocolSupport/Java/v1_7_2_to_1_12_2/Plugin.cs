using System.Buffers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Network.IO.Channels.Services;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins;
using Void.Proxy.Common.Network.IO.Channels.Services;
using Void.Proxy.Common.Network.IO.Channels;
using Void.Proxy.Common.Network.IO.Messages.Binary;
using Void.Proxy.Common.Network.IO.Streams.Compression;
using Void.Proxy.Common.Network.IO.Streams.Encryption;
using Void.Proxy.Common.Network.IO.Streams.Network;
using Void.Proxy.Common.Network.IO.Streams.Packet;
using Void.Proxy.Common.Network.Protocol;
using Void.Proxy.Common.Registries.Packets;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Registries;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2;

public class Plugin(ILogger<Plugin> logger, IPlayerService players) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_7_2, ProtocolVersion.MINECRAFT_1_12_2);

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
            case LoginSuccessPacket loginSuccess:
                holder = @event.Link.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();
                holder.ReplacePackets(Direction.Serverbound, Mappings.ServerboundPlayMappings);
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
            case LoginSuccessPacket loginSuccess:
                holder = @event.Link.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();
                holder.ReplacePackets(Direction.Clientbound, Mappings.ClientboundPlayMappings);
                break;
            case EncryptionRequestPacket encryptionRequest:
                logger.LogDebug(JsonSerializer.Serialize(encryptionRequest));
                break;
            case EncryptionResponsePacket encryptionResponse:
                var serverPrivateKey = Convert.FromHexString("30820276020100300d06092a864886f70d0101010500048202603082025c0201000281810087394b97103c1ec8e7540e84fcf1347ceaac1687ab85e1c3c944b3b99f3ecbcd79143a544e32f917db06280208c2631648e7b357bad4e263cdfd186e3d236c2fef59e6bf35afd3117de9a87667b770c0b2076f44ac84b10d2733f34eb64c5b4bbd640f278faf79407150292173704ae0df08d124eeaa6771e6fcb763eff47e630203010001028180371382133c3c907959a725e6cab35fa2855ea42a8c1512b850d0d7b702026cb5fadbdf4a1933afbb24ad643c37df1cc6a5d92175d915fe439251c4d299ba44c4c2f28058cd04482b00ffd46fafdda7de603e07ce025c56a4e14b900f02d37cb4464babc14f9bfcd42cfb00f24963eaa83af2c56288908fb915a8ba6f6fd3ab91024100e203bbfb95a6320882ebc1858d5099059bac1ffd3a2164de5f89a9b4dfb1c2eacf2e7923a28f2c72e1e67ef4646dd2ed90538c31c0c23943bd6468100a4b510b0241009929fb0c628fc0360de7ea9beaa02882f3f1245ee4e2c8d1b456058f6a1c6d3e22d544902a434f92b47805c0d11272280552435adf636cef3eb49c51a40d0f090241009a3a341fc489a960b77e0b0b8857f463a84bf77444239f82432d5c59e9bf92a3ca870af0ef2fc6040af4cfb3138901c34c96467778f2d042d24d5ed97b3cd3eb02400707a63e2b47edca8d58d7abd0590982f92f583c02c565f23a14b8ac9c7231916887e15dbc92da54217460cf38c95ff3f64a904cdb73f4cc0654c3d7fb6f9e01024057cfc7a244c458804cc8836284815efa717b23830414a443ecd264ca6ef9b70ff4afd24fb60e6a5466ddb90ece95940d5859aa5576e57f228657fd66a654cf61");

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