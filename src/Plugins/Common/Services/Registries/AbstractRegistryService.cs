using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Minecraft;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Links;
using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Channels.Extensions;
using Void.Proxy.API.Network.IO.Messages.Binary;
using Void.Proxy.API.Network.IO.Messages.Packets;
using Void.Proxy.API.Network.IO.Streams.Packet;
using Void.Proxy.API.Network.IO.Streams.Packet.Registries;
using Void.Proxy.API.Network.IO.Streams.Recyclable;
using Void.Proxy.API.Players;
using Void.Proxy.API.Players.Extensions;
using Void.Proxy.API.Plugins;
using Void.Proxy.Plugins.Common.Extensions;

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

        channel.ClearPacketsMappings(plugin);
    }

    [Subscribe(PostOrder.First)]
    public static async ValueTask OnPhaseChanged(PhaseChangedEvent @event, CancellationToken cancellationToken)
    {
        // At handshake phase IPlayer channel is still being built, causing stack overflow here
        if (@event.Phase is Phase.Handshake)
            return;

        await @event.Player.ClearPluginsPacketRegistryAsync(cancellationToken);
    }

    [Subscribe(PostOrder.Last)]
    public async ValueTask OnMessageReceivedWithCustomRegistry(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Link.Player.ProtocolVersion))
            return;

        var registries = await @event.Link.Player.GetPluginsPacketRegistriesAsync(cancellationToken);

        if (registries.IsEmpty)
            return;

        if (registries.Contains(@event.Message))
            return;

        try
        {
            var packets = @event.Message switch
            {
                IBinaryMessage binaryMessage => DecodeBinaryMessage(@event.Link, registries, binaryMessage),
                IMinecraftPacket minecraftPacket => DecodeMinecraftPacket(@event.Link, registries, minecraftPacket),
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

        var registries = await @event.Link.Player.GetPluginsPacketRegistriesAsync(cancellationToken);

        if (registries.IsEmpty)
            return;

        if (registries.Contains(@event.Message))
            return;

        try
        {
            var packets = @event.Message switch
            {
                IBinaryMessage binaryMessage => DecodeBinaryMessage(@event.Link, registries, binaryMessage),
                IMinecraftPacket minecraftPacket => DecodeMinecraftPacket(@event.Link, registries, minecraftPacket),
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
                link.ServerChannel.ClearPacketsMappings(plugin);

            channel.ClearPacketsMappings(plugin);
        }
    }

    protected static IEnumerable<IMinecraftPacket> DecodeBinaryMessage(ILink link, IMinecraftPacketRegistryPlugins registries, IBinaryMessage binaryMessage)
    {
        foreach (var registry in registries.All)
        {
            if (!registry.TryCreateDecoder(binaryMessage.Id, out var decoder))
                continue;

            var position = binaryMessage.Stream.Position;

            var buffer = new MinecraftBuffer(binaryMessage.Stream);
            var packet = decoder(ref buffer, link.Player.ProtocolVersion);

            binaryMessage.Stream.Position = position;

            yield return packet;
        }
    }

    protected static IEnumerable<IMinecraftPacket> DecodeMinecraftPacket(ILink link, IMinecraftPacketRegistryPlugins registries, IMinecraftPacket minecraftPacket)
    {
        var playerPacketRegistryHolder = link.PlayerChannel.GetSystemPacketRegistryHolder();
        var serverPacketRegistryHolder = link.ServerChannel.GetSystemPacketRegistryHolder();

        if (!playerPacketRegistryHolder.Read.TryGetPacketId(minecraftPacket, out var id) &&
            !playerPacketRegistryHolder.Write.TryGetPacketId(minecraftPacket, out id) &&
            !serverPacketRegistryHolder.Read.TryGetPacketId(minecraftPacket, out id) &&
            !serverPacketRegistryHolder.Write.TryGetPacketId(minecraftPacket, out id))
            yield break;

        foreach (var registry in registries.All)
        {
            if (!registry.TryCreateDecoder(id, out var decoder))
                continue;

            using var stream = MinecraftRecyclableStream.RecyclableMemoryStreamManager.GetStream();
            var buffer = new MinecraftBuffer(stream);

            minecraftPacket.Encode(ref buffer, link.Player.ProtocolVersion);
            buffer.Reset();

            yield return decoder(ref buffer, link.Player.ProtocolVersion);
        }
    }

    protected abstract bool IsSupportedVersion(ProtocolVersion version);
}