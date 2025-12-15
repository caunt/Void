using Microsoft.Extensions.Logging;
using Void.Minecraft.Buffers;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Channels.Extensions;
using Void.Minecraft.Network.Messages.Binary;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries.PacketId;
using Void.Minecraft.Network.Registries.PacketId.Extensions;
using Void.Minecraft.Network.Registries.Transformations;
using Void.Minecraft.Network.Registries.Transformations.Mappings;
using Void.Minecraft.Network.Streams.Packet;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Channels;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Network.Streams.Recyclable;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.Messages.Binary;
using Void.Proxy.Plugins.Common.Network.Registries.Transformations.Mappings;

namespace Void.Proxy.Plugins.Common.Services.Registries;

public abstract class AbstractRegistryService(ILogger<AbstractRegistryService> logger, IPlugin plugin, IPlayerService players, ILinkService links, IEventService events) : IPluginCommonService
{
    [Subscribe(PostOrder.First)]
    public void OnPluginUnloaded(PluginUnloadedEvent @event)
    {
        foreach (var player in players.All)
        {
            if (!player.TryGetLink(out var link))
                continue;

            link.PlayerChannel.GetMinecraftRegistries().ClearPlugin(@event.Plugin);
            link.ServerChannel.GetMinecraftRegistries().ClearPlugin(@event.Plugin);
        }
    }

    [Subscribe]
    public async ValueTask OnChannelCreated(ChannelCreatedEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        SetupRegistries(@event.Channel, @event.Side, @event.Player.ProtocolVersion);
        await @event.Player.SetPhaseAsync(@event.Side, Phase.Handshake, @event.Channel, cancellationToken);
    }

    [Subscribe]
    public void OnLinkStoppingEvent(LinkStoppingEvent @event)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        if (@event.Link.PlayerChannel.TryGet<IMinecraftPacketMessageStream>(out var playerPacketStream))
        {
            playerPacketStream.Registries.PacketTransformationsSystem.Clear();
            playerPacketStream.Registries.PacketTransformationsPlugins.Clear();
        }

        if (@event.Link.ServerChannel.TryGet<IMinecraftPacketMessageStream>(out var serverPacketStream))
        {
            serverPacketStream.Registries.PacketTransformationsSystem.Clear();
            serverPacketStream.Registries.PacketTransformationsPlugins.Clear();
        }
    }

    [Subscribe(PostOrder.First)]
    public void OnPhaseChanged(PhaseChangedEvent @event)
    {
        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        // At handshake phase IPlayer channel is still being built, causing stack overflow here
        if (@event.Phase is Phase.Handshake)
            return;

        var link = @event.Player.GetLink();

        // Clear only affected registries that are no longer valid
        if (@event.Side is Side.Client)
        {
            link.PlayerChannel.GetMinecraftRegistries().ClearPlugins(Direction.Clientbound, Operation.Any);
            link.PlayerChannel.GetMinecraftRegistries().ClearPlugins(Direction.Serverbound, Operation.Any);
            link.ServerChannel.GetMinecraftRegistries().ClearPlugins(Direction.Clientbound, Operation.Read);
            link.ServerChannel.GetMinecraftRegistries().ClearPlugins(Direction.Serverbound, Operation.Read);
        }

        if (@event.Side is Side.Server)
        {
            link.ServerChannel.GetMinecraftRegistries().ClearPlugins(Direction.Clientbound, Operation.Any);
            link.ServerChannel.GetMinecraftRegistries().ClearPlugins(Direction.Serverbound, Operation.Any);
            link.PlayerChannel.GetMinecraftRegistries().ClearPlugins(Direction.Clientbound, Operation.Read);
            link.PlayerChannel.GetMinecraftRegistries().ClearPlugins(Direction.Serverbound, Operation.Read);
        }
    }

    [Subscribe]
    public async ValueTask OnMessageReceivedWithCustomRegistry(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        var operation = Operation.Read;
        var channel = @event.Direction switch
        {
            Direction.Clientbound => @event.Link.ServerChannel,
            Direction.Serverbound => @event.Link.PlayerChannel,
            _ => throw new InvalidOperationException($"Unknown direction {@event.Direction}")
        };

        var registries = channel.GetMinecraftRegistries().PacketIdPlugins;

        if (registries.IsEmpty)
            return;

        if (registries.Contains(@event.Message))
            return;

        var transformations = channel.GetMinecraftRegistries().PacketTransformationsPlugins;

        try
        {
            var packets = @event.Message switch
            {
                IMinecraftBinaryMessage binaryMessage => DecodeBinaryMessage(@event.Link, @event.Origin, operation, registries, transformations, binaryMessage),
                IMinecraftPacket minecraftPacket => DecodeMinecraftPacket(@event.Link, @event.Origin, operation, registries, transformations, minecraftPacket),
                _ => null
            };

            if (packets is null)
                return;

            foreach (var packet in packets)
                @event.Result = await events.ThrowWithResultAsync(new MessageReceivedEvent(@event.Origin, @event.From, @event.To, @event.Direction, packet, @event.Link, @event.Player), cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error decoding or handling {Type}", @event.Message);
        }
    }

    [Subscribe]
    public async ValueTask OnMessageSentWithCustomRegistry(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        var operation = Operation.Write;
        var channel = @event.Direction switch
        {
            Direction.Clientbound => @event.Link.PlayerChannel,
            Direction.Serverbound => @event.Link.ServerChannel,
            _ => throw new InvalidOperationException($"Unknown direction {@event.Direction}")
        };

        var registries = channel.GetMinecraftRegistries().PacketIdPlugins;

        if (registries.IsEmpty)
            return;

        if (registries.Contains(@event.Message))
            return;

        var transformations = channel.GetMinecraftRegistries().PacketTransformationsPlugins;

        try
        {
            var packets = @event.Message switch
            {
                IMinecraftBinaryMessage binaryMessage => DecodeBinaryMessage(@event.Link, @event.Origin, operation, registries, transformations, binaryMessage),
                IMinecraftPacket minecraftPacket => DecodeMinecraftPacket(@event.Link, @event.Origin, operation, registries, transformations, minecraftPacket),
                _ => null
            };

            if (packets is null)
                return;

            foreach (var packet in packets)
                await events.ThrowAsync(new MessageSentEvent(@event.Origin, @event.From, @event.To, @event.Direction, packet, @event.Link, @event.Player), cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error decoding or handling {Type}", @event.Message);
        }
    }

    [Subscribe]
    public async ValueTask OnPluginUnloading(PluginUnloadingEvent @event, CancellationToken cancellationToken)
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
                link.ServerChannel.DisposeRegistries(plugin);

            channel.DisposeRegistries(plugin);
        }
    }

    protected static IEnumerable<IMinecraftPacket> DecodeBinaryMessage(ILink link, Side origin, Operation operation, IMinecraftPacketIdPluginsRegistry registries, IMinecraftPacketTransformationsPluginsRegistry transformationsMappings, IMinecraftBinaryMessage binaryMessage)
    {
        if (!link.Player.IsMinecraft)
            yield break;

        var filteredRegistries = operation switch
        {
            Operation.Read => registries.Read,
            Operation.Write => registries.Write,
            _ => []
        };

        var queue = new Queue<IMinecraftPacketIdRegistry>(filteredRegistries);
        while (queue.TryDequeue(out var registry))
        {
            if (!registry.TryCreateDecoder(binaryMessage.Id, out var type, out var decoder))
                continue;

            // Do not pass origin binary message to transformers, so plugins cannot modify it
            var position = binaryMessage.Stream.Position;

            using var stream = RecyclableStream.RecyclableMemoryStreamManager.GetStream();
            binaryMessage.Stream.CopyTo(stream);
            binaryMessage.Stream.Position = position;

            stream.Position = 0;
            var buffer = new MinecraftBuffer(stream);
            var wrapper = new MinecraftBinaryPacketWrapper(new MinecraftBinaryPacket(binaryMessage.Id, stream), origin);

            if (registries.TryGetTransformations(transformationsMappings, type, TransformationType.Upgrade, out var transformations))
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

            yield return packet;
        }
    }

    protected static IEnumerable<IMinecraftPacket> DecodeMinecraftPacket(ILink link, Side origin, Operation operation, IMinecraftPacketIdPluginsRegistry registries, IMinecraftPacketTransformationsPluginsRegistry transformationsMappings, IMinecraftPacket minecraftPacket)
    {
        if (!link.Player.IsMinecraft)
            yield break;

        var playerRegistry = link.PlayerChannel.GetMinecraftRegistries().PacketIdSystem;
        var serverRegistry = link.ServerChannel.GetMinecraftRegistries().PacketIdSystem;

        if (!playerRegistry.Write.TryGetPacketId(minecraftPacket, out var id) &&
            !serverRegistry.Write.TryGetPacketId(minecraftPacket, out id) &&
            !playerRegistry.Read.TryGetPacketId(minecraftPacket, out id) &&
            !serverRegistry.Read.TryGetPacketId(minecraftPacket, out id))
            yield break;

        var filteredRegistries = operation switch
        {
            Operation.Read => registries.Read,
            Operation.Write => registries.Write,
            _ => []
        };

        var queue = new Queue<IMinecraftPacketIdRegistry>(filteredRegistries);
        while (queue.TryDequeue(out var registry))
        {
            if (!registry.TryCreateDecoder(id, out var type, out var decoder))
                continue;

            using var stream = RecyclableStream.RecyclableMemoryStreamManager.GetStream();
            var buffer = new MinecraftBuffer(stream);
            var wrapper = new MinecraftBinaryPacketWrapper(new MinecraftBinaryPacket(id, stream), origin);

            minecraftPacket.Encode(ref buffer, link.Player.ProtocolVersion);
            stream.Position = 0;

            if (registries.TryGetTransformations(transformationsMappings, type, TransformationType.Upgrade, out var transformations))
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
    protected abstract void SetupRegistries(INetworkChannel channel, Side side, ProtocolVersion protocolVersion);
}
