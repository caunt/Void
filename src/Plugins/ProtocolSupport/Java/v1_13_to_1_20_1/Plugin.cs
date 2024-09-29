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
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Serverbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Registries;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1;

public class Plugin(ILogger<Plugin> logger, IPlayerService players) : IProtocolPlugin
{
    public static IEnumerable<ProtocolVersion> SupportedVersions => ProtocolVersion.Range(ProtocolVersion.MINECRAFT_1_13, ProtocolVersion.MINECRAFT_1_20);

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
                var serverPrivateKey = Convert.FromHexString("30820275020100300d06092a864886f70d01010105000482025f3082025b020100028181009e50fbb64abd26763ee34e599c5ca1237f9d1366d8f0bb5ef0fa8b1d60c66eba2b17b45225697b03a378b674b72da7eca58341ddb1f7cb5a17bec9afbb3a626e85bfa4c13b3a3e83dc9a259fa6010e3dd5502c94d07ef36746c756a6b4e1d5e5fadae3a21ca9a58a8ff1e30f83487ccc21591b7cdff5996b0ffaee1268f269dd0203010001028180082522fa2338bd8e965833fafcc6303d270ee20567d264d572e0f500cf2e8e5a5226bf4e7da1c26432c81e5cfd62a231d4e71e9fa50d1a5f797ac59e1eac178dc2b89d42c057805675837f0c82c34ebc1350af7d7aeab6545d23ec89a891daadb7374e816c7519d655be3feb3dad8eaab2be3567b061597c62f266142624a281024100afd650e871b1fe3034298c7a644ccb7bd6d8acf5cf5ab9b80c151a502e0592c9f628084844d11a36287bfd6347b47cdcd7ea4cd443f88b6d1e346ac3348abd5d024100e67dd68fd5a45e2b165d14acc72ccf91cbfeadc07e92ec988c8a5c8fd78c771324a473c5955f27e45055bfe69c966c1207849fdbd16af27d3cdcd05e9e9f168102401055905e87714973a0e4db2fe3715ed5ba3796999f11867f1a603b79874ced3de5a8025cf447986fbf83051edbe260570397b85b5f950f1eabd7a2b34f8633e102400abda678016ead6cb89811d4935b8538b816e025ee400e82755a254216bb56f92f65f2772ca4c8891d2d41ca0f55fb2743ee6c3ca41fa5b92c52e5eae2dd6b0102407cde5685a5021672657bada13b5f8b12525c93234f33cd3739442c63378fb4790f6ffbf2f7974ae7204db77d9ea6d110ec3242f28ae42b7e70850fbb59807afd");

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