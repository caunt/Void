using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Channels;
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

        logger.LogTrace("Received packet {Packet}", @event.Message);

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
                logger.LogTrace("Link {Link} enabled compression in server channel with threshold {CompressionThreshold}", @event.Link, setCompression.Threshold);

                var zlibStream = @event.Link.ServerChannel.Get<SharpZipLibCompressionMessageStream>();
                zlibStream.CompressionThreshold = setCompression.Threshold;
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
            case LoginStartPacket loginStart:
                @event.Link.PlayerChannel.Pause();
                break;
            case BinaryPacket binaryPacket:
                logger.LogTrace("Sent packet id {PacketId:X2}, length {Length} to {Direction} {PlayerOrServer}", binaryPacket.Id, binaryPacket.Stream.Length, @event.To, @event.To == Side.Client ? @event.Link.Player : @event.Link.Server);
                return;
        }

        logger.LogTrace("Sent packet {Packet}", @event.Message);

        switch (@event.Message)
        {
            case HandshakePacket handshake:
                var holder = @event.Link.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();

                if (handshake.NextState == 2)
                    holder.ReplacePackets(Direction.Serverbound, Mappings.ServerboundLoginMappings);
                else
                    holder.Reset();
                break;
            case SetCompressionPacket setCompression:
                @event.Link.PlayerChannel.AddBefore<MinecraftPacketMessageStream, SharpZipLibCompressionMessageStream>();

                var zlibStream = @event.Link.PlayerChannel.Get<SharpZipLibCompressionMessageStream>();
                zlibStream.CompressionThreshold = setCompression.Threshold;

                logger.LogTrace("Link {Link} enabled compression in player channel with threshold {CompressionThreshold}", @event.Link, setCompression.Threshold);
                @event.Link.PlayerChannel.Resume();
                break;
            case LoginSuccessPacket loginSuccess:
                holder = @event.Link.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();
                holder.ReplacePackets(Direction.Clientbound, Mappings.ClientboundPlayMappings);

                if (@event.Link.PlayerChannel.IsPaused)
                    @event.Link.PlayerChannel.Resume();
                break;
            case EncryptionRequestPacket encryptionRequest:
                @event.Link.PlayerChannel.Resume();
                @event.Link.ServerChannel.Pause();
                break;
            case EncryptionResponsePacket encryptionResponse:
                var serverPrivateKey = Convert.FromHexString("30820275020100300d06092a864886f70d01010105000482025f3082025b020100028181008833058bd694928c004a7ffbaeca35f5bac94b070082080958c71bc8bc23a507c64432a8d38cb3d405928ef58f89f681c87a3909f3a1ec08cbdc5223efb612efcdeb639fd93562ac73e407c6bd8d68d7dc1a226e6c95c9f3e5a94edaec76a98c411caf6caec3f2618ea33169b72e42687faf4ac7478af722df6c234b5fcc82ef0203010001028180119dc27a8dd74b052a07237aa2e700b261e5d0a5d288ff0fc66dd51d7cf2d750f97204b8c01ebe26644f75323478a71658f486157ac5021f392456c8d323d25b0583aa927c9bf835017fe3af24da1c4dd6ab416c06bbedfef40ef4428adc60d4e2d5e613d3278ca87590fc91ff7d43ad359cf16215af0391e53eef1581882dc1024100a99aca2367811e10f8e15ee5472d50507756bb48c5be1cc5cb6251b43dcec7163f52913766722968d1843daf99687026010db8986ad1c58005c238eec2ca2bff024100cd940653037a7ff8e8e4aa3d1d6d543a12297538dac4c4def496fd96d030dd0c0daa32da80b14e1344a8571e140ce0d27e009cbe0a7e9124a0a90a2e0bac691102401ddc048e6b208e3c8ab492d266cf917e392469e08bffc66d043b910adc7ed50a13a7e3ad0f3a3614201eda055a4acac3c617b6520f2c534b10b87af17e15bddd023f3c3a21a03064b3193921c4be22e0e4cc1e8606d1a14604674d40ef0a3ff410ce773265b39e0053df513e0047cf97f645b4a4794733cbe0b9da57aba3d1c7b10241009905827e05808b57c0c836d29b1efe9852f502509b54ab6246539d359cbb036904283b6f6f5636659f7b25478949f0acc3a7cf4edf13cd2539376247845deff1");

                var secret = Decrypt(serverPrivateKey, encryptionResponse.SharedSecret);

                @event.Link.ServerChannel.AddBefore<MinecraftPacketMessageStream, AesCfb8BufferedStream>(new AesCfb8BufferedStream(secret));
                @event.Link.PlayerChannel.AddBefore<MinecraftPacketMessageStream, AesCfb8BufferedStream>(new AesCfb8BufferedStream(secret));

                @event.Link.PlayerChannel.Pause();
                @event.Link.ServerChannel.Resume();
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