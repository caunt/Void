using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Minecraft;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Links;
using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Channels.Extensions;
using Void.Proxy.API.Network.IO.Messages.Binary;
using Void.Proxy.API.Network.IO.Messages.Packets;
using Void.Proxy.API.Network.IO.Streams.Packet;
using Void.Proxy.API.Network.IO.Streams.Recyclable;
using Void.Proxy.API.Players;
using Void.Proxy.API.Players.Extensions;
using Void.Proxy.API.Plugins;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Packet;

namespace Void.Proxy.Plugins.Common.Services.Registries;

public abstract class AbstractRegistryService(ILogger<AbstractRegistryService> logger, IPlugin plugin, IPlayerService players, IEventService events) : IPluginService
{
    [Subscribe]
    public static void OnPlayerConnecting(PlayerConnectingEvent @event)
    {
        if (!@event.Services.HasService<IMinecraftPacketRegistry>())
            @event.Services.AddSingleton<IMinecraftPacketRegistry, MinecraftPacketRegistry>();
    }

    [Subscribe]
    public void OnPlayerDisconnected(PlayerDisconnectedEvent @event)
    {
        var channel = @event.Player.Context.Channel;

        if (channel is null)
            return;

        if (!channel.TryGet<IMinecraftPacketMessageStream>(out _))
            return;

        channel.ClearPacketsMappings(plugin);
    }

    [Subscribe(PostOrder.First)]
    public static void OnPhaseChanged(PhaseChangedEvent @event)
    {
        @event.Player.ClearPacketRegistry();
    }

    [Subscribe(PostOrder.Last)]
    public async ValueTask OnMessageReceivedWithCustomRegistry(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Link.Player.ProtocolVersion))
            return;

        var registry = @event.Link.Player.GetPacketRegistry();

        if (registry.IsEmpty)
            return;

        if (registry.Contains(@event.Message))
            return;

        try
        {
            var packet = @event.Message switch
            {
                IBinaryMessage binaryMessage => DecodeBinaryMessage(@event.Link, registry, binaryMessage),
                IMinecraftPacket minecraftPacket => DecodeMinecraftPacket(@event.Link, registry, minecraftPacket),
                _ => null
            };

            if (packet is null)
                return;

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

        var registry = @event.Link.Player.GetPacketRegistry();

        if (registry.IsEmpty)
            return;

        if (registry.Contains(@event.Message))
            return;

        try
        {
            var packet = @event.Message switch
            {
                IBinaryMessage binaryMessage => DecodeBinaryMessage(@event.Link, registry, binaryMessage),
                IMinecraftPacket minecraftPacket => DecodeMinecraftPacket(@event.Link, registry, minecraftPacket),
                _ => null
            };

            if (packet is null)
                return;

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

            channel.ClearPacketsMappings(plugin);
        }
    }

    protected static IMinecraftPacket? DecodeBinaryMessage(ILink link, IMinecraftPacketRegistry registry, IBinaryMessage binaryMessage)
    {
        if (!registry.TryCreateDecoder(binaryMessage.Id, out var decoder))
            return null;

        var position = binaryMessage.Stream.Position;

        var buffer = new MinecraftBuffer(binaryMessage.Stream);
        var packet = decoder(ref buffer, link.Player.ProtocolVersion);

        binaryMessage.Stream.Position = position;

        return packet;
    }

    protected static IMinecraftPacket? DecodeMinecraftPacket(ILink link, IMinecraftPacketRegistry registry, IMinecraftPacket minecraftPacket)
    {
        var playerPacketRegistryHolder = link.PlayerChannel.GetPacketRegistryHolder();
        var serverPacketRegistryHolder = link.ServerChannel.GetPacketRegistryHolder();

        if (!playerPacketRegistryHolder.Read.TryGetPacketId(minecraftPacket, out var id) &&
            !playerPacketRegistryHolder.Write.TryGetPacketId(minecraftPacket, out id) &&
            !serverPacketRegistryHolder.Read.TryGetPacketId(minecraftPacket, out id) &&
            !serverPacketRegistryHolder.Write.TryGetPacketId(minecraftPacket, out id))
            return null;

        if (!registry.TryCreateDecoder(id, out var decoder))
            return null;

        using var stream = MinecraftRecyclableStream.RecyclableMemoryStreamManager.GetStream();
        var buffer = new MinecraftBuffer(stream);

        minecraftPacket.Encode(ref buffer, link.Player.ProtocolVersion);
        buffer.Reset();

        return decoder(ref buffer, link.Player.ProtocolVersion);
    }

    protected abstract bool IsSupportedVersion(ProtocolVersion version);
}