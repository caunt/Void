using Microsoft.Extensions.DependencyInjection;
using Void.Common;
using Void.Minecraft.Network.Messages.Packets;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Network.IO.Channels.Extensions;
using Void.Proxy.Api.Network.IO.Streams.Packet.Registries;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;

namespace Void.Proxy.Api.Links.Extensions;

public static class MinecraftLinkExtensions
{
    public static async ValueTask SendPacketAsync<T>(this ILink link, CancellationToken cancellationToken) where T : class, IMinecraftPacket, new()
    {
        await link.SendPacketAsync(new T(), cancellationToken);
    }

    public static async ValueTask SendPacketAsync<T>(this ILink link, T packet, CancellationToken cancellationToken) where T : class, IMinecraftPacket
    {
        await link.SendPacketAsync(packet is IMinecraftClientboundPacket ? Side.Client : Side.Server, packet, cancellationToken);
    }

    public static async ValueTask SendPacketAsync<T>(this ILink link, Side side, T packet, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        if (side is Side.Proxy)
            throw new InvalidOperationException("What do you mean by sending packet to proxy side?");

        var channel = side is Side.Client ? link.PlayerChannel : link.ServerChannel;
        await channel.SendPacketAsync(packet, cancellationToken);

        var events = link.Player.Context.Services.GetRequiredService<IEventService>();
        var direction = side is Side.Client ? Direction.Clientbound : Direction.Serverbound;
        await events.ThrowAsync(new MessageSentEvent(Side.Proxy, Side.Proxy, side, direction, packet, link), cancellationToken);
    }

    public static IMinecraftPacketPluginsRegistry GetPacketPluginsRegistries(this ILink link, Direction direction)
    {
        var channel = link.GetChannel(direction);
        return channel.GetPacketPluginsRegistryHolder();
    }

    public static IMinecraftPacketPluginsTransformations GetPacketPluginsTransformations(this ILink link, Direction direction)
    {
        var channel = link.GetChannel(direction);
        return channel.GetPacketTransformationsHolder();
    }

    public static IMinecraftChannel GetChannel(this ILink link, Direction direction)
    {
        return direction switch
        {
            Direction.Clientbound => link.PlayerChannel,
            Direction.Serverbound => link.ServerChannel,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}
