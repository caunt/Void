using Microsoft.Extensions.Logging;
using Void.Minecraft.Buffers;
using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Minecraft;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Links.Extensions;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.IO.Channels.Extensions;
using Void.Proxy.Api.Network.IO.Messages.Binary;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Network.IO.Streams.Packet;
using Void.Proxy.Api.Network.IO.Streams.Packet.Extensions;
using Void.Proxy.Api.Network.IO.Streams.Packet.Registries;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;
using Void.Proxy.Api.Network.IO.Streams.Recyclable;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.IO.Messages.Binary;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Packet.Transformations;

namespace Void.Proxy.Plugins.Common.Services.Registries;

public abstract class AbstractRegistryService(ILogger<AbstractRegistryService> logger, IPlugin plugin, IPlayerService players, ILinkService links, IEventService events) : IPluginCommonService
{
    [Subscribe]
    public void OnPlayerDisconnected(PlayerDisconnectedEvent @event)
    {
        var channel = @event.Player.Context.Channel;

        if (channel is null)
            return;

        if (!channel.TryGet<IMinecraftPacketMessageStream>(out _))
            return;

        channel.ClearPluginsHolders(plugin);
    }

    [Subscribe(PostOrder.First)]
    public static void OnPhaseChanged(PhaseChangedEvent @event)
    {
        // At handshake phase IPlayer channel is still being built, causing stack overflow here
        if (@event.Phase is Phase.Handshake)
            return;

        var link = @event.Player.GetLink();

        link.GetPacketPluginsRegistries(Direction.Serverbound).Clear();
        link.GetPacketPluginsRegistries(Direction.Clientbound).Clear();

        link.GetPacketPluginsTransformations(Direction.Serverbound).Clear();
        link.GetPacketPluginsTransformations(Direction.Clientbound).Clear();
    }

    [Subscribe(PostOrder.Last)]
    public async ValueTask OnMessageReceivedWithCustomRegistry(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Link.Player.ProtocolVersion))
            return;

        var registries = @event.Link.GetPacketPluginsRegistries(@event.Direction);

        if (registries.IsEmpty)
            return;

        if (registries.Contains(@event.Message))
            return;

        var transformations = @event.Link.GetPacketPluginsTransformations(@event.Direction);

        try
        {
            var packets = @event.Message switch
            {
                IMinecraftBinaryMessage binaryMessage => DecodeBinaryMessage(@event.Link, registries, transformations, binaryMessage),
                IMinecraftPacket minecraftPacket => DecodeMinecraftPacket(@event.Link, registries, transformations, minecraftPacket),
                _ => null
            };

            if (packets is null)
                return;

            foreach (var packet in packets)
                @event.Result = await events.ThrowWithResultAsync(new MessageReceivedEvent(@event.Origin, @event.From, @event.To, @event.Direction, packet, @event.Link), cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error decoding or handling {Type}", @event.Message);
        }
    }

    [Subscribe(PostOrder.Last)]
    public async ValueTask OnMessageSentWithCustomRegistry(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Link.Player.ProtocolVersion))
            return;

        var registries = @event.Link.GetPacketPluginsRegistries(@event.Direction);

        if (registries.IsEmpty)
            return;

        if (registries.Contains(@event.Message))
            return;

        var transformations = @event.Link.GetPacketPluginsTransformations(@event.Direction);

        try
        {
            var packets = @event.Message switch
            {
                IMinecraftBinaryMessage binaryMessage => DecodeBinaryMessage(@event.Link, registries, transformations, binaryMessage),
                IMinecraftPacket minecraftPacket => DecodeMinecraftPacket(@event.Link, registries, transformations, minecraftPacket),
                _ => null
            };

            if (packets is null)
                return;

            foreach (var packet in packets)
                await events.ThrowAsync(new MessageSentEvent(@event.Origin, @event.From, @event.To, @event.Direction, packet, @event.Link), cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error decoding or handling {Type}", @event.Message);
        }
    }

    [Subscribe]
    public async ValueTask OnPluginUnload(PluginUnloadEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Plugin != plugin)
            return;

        events.UnregisterListeners(this);

        foreach (var player in players.All)
        {
            var channel = await player.GetChannelAsync(cancellationToken);

            if (!channel.TryGet<IMinecraftPacketMessageStream>(out _))
                continue;

            if (links.TryGetLink(player, out var link))
                link.ServerChannel.ClearPluginsHolders(plugin);

            channel.ClearPluginsHolders(plugin);
        }
    }

    protected static IEnumerable<IMinecraftPacket> DecodeBinaryMessage(ILink link, IMinecraftPacketPluginsRegistry registries, IMinecraftPacketPluginsTransformations transformationsMappings, IMinecraftBinaryMessage binaryMessage)
    {
        foreach (var registry in registries.All)
        {
            if (!registry.TryCreateDecoder(binaryMessage.Id, out var type, out var decoder))
                continue;

            var position = binaryMessage.Stream.Position;
            var buffer = new MinecraftBuffer(binaryMessage.Stream);
            var wrapper = new MinecraftBinaryPacketWrapper(binaryMessage);

            if (registries.TryGetTransformations(transformationsMappings, type, out var transformations))
            {
                foreach (var transformation in transformations)
                {
                    transformation(wrapper);
                    wrapper.Reset();
                    binaryMessage.Stream.Position = position;
                }
            }

            wrapper.WriteProcessedValues(ref buffer);

            binaryMessage.Stream.SetLength(binaryMessage.Stream.Position);
            binaryMessage.Stream.Position = position;

            var packet = decoder(ref buffer, link.Player.ProtocolVersion);

            binaryMessage.Stream.Position = position;
            yield return packet;
        }
    }

    protected static IEnumerable<IMinecraftPacket> DecodeMinecraftPacket(ILink link, IMinecraftPacketPluginsRegistry registries, IMinecraftPacketPluginsTransformations transformationsMappings, IMinecraftPacket minecraftPacket)
    {
        var playerPacketRegistryHolder = link.PlayerChannel.GetPacketSystemRegistryHolder();
        var serverPacketRegistryHolder = link.ServerChannel.GetPacketSystemRegistryHolder();

        if (!playerPacketRegistryHolder.Write.TryGetPacketId(minecraftPacket, out var id) && !serverPacketRegistryHolder.Write.TryGetPacketId(minecraftPacket, out id))
            yield break;

        foreach (var registry in registries.All)
        {
            if (!registry.TryCreateDecoder(id, out var type, out var decoder))
                continue;

            using var stream = MinecraftRecyclableStream.RecyclableMemoryStreamManager.GetStream();
            var buffer = new MinecraftBuffer(stream);
            var wrapper = new MinecraftBinaryPacketWrapper(new MinecraftBinaryPacket(id, stream));

            minecraftPacket.Encode(ref buffer, link.Player.ProtocolVersion);
            stream.Position = 0;

            if (registries.TryGetTransformations(transformationsMappings, type, out var transformations))
            {
                foreach (var transformation in transformations)
                {
                    transformation(wrapper);
                    wrapper.Reset();
                    stream.Position = 0;
                }
            }

            wrapper.WriteProcessedValues(ref buffer);

            stream.SetLength(stream.Position);
            stream.Position = 0;

            var packet = decoder(ref buffer, link.Player.ProtocolVersion);

            stream.Position = 0;
            yield return packet;
        }
    }

    protected abstract bool IsSupportedVersion(ProtocolVersion version);
}