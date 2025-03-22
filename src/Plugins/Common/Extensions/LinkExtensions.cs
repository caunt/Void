using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Links;
using Void.Proxy.API.Network;
using Void.Proxy.Plugins.Common.Network.IO.Bundles;
using Void.Proxy.Plugins.Common.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Packets;

namespace Void.Proxy.Plugins.Common.Extensions;

public static class LinkExtensions
{
    public static async ValueTask<T> ReceivePacketAsync<T>(this ILink link, CancellationToken cancellationToken) where T : class, IMinecraftPacket
    {
        var type = typeof(T);

        if (type == typeof(IMinecraftPacket))
            throw new ArgumentException("You should specify any side-bound packet type");

        var side = type.IsAssignableTo(typeof(IClientboundPacket)) ? Side.Server : Side.Client;

        return await link.ReceivePacketAsync<T>(side, cancellationToken);
    }

    public static async ValueTask<T> ReceivePacketAsync<T>(this ILink link, Side side, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        if (side is Side.Proxy)
            throw new InvalidOperationException("How would I read packet from proxy?");

        var channel = side is Side.Client ? link.PlayerChannel : link.ServerChannel;
        var packet = await channel.ReceivePacketAsync<T>(cancellationToken);

        var events = link.Player.Context.Services.GetRequiredService<IEventService>();
        var cancelled = await events.ThrowWithResultAsync(new MessageReceivedEvent
        {
            Origin = side,
            Direction = side switch
            {
                Side.Client => Direction.Serverbound,
                Side.Server => Direction.Clientbound,
                _ => throw new InvalidOperationException(nameof(side))
            },
            From = side,
            To = Side.Proxy,
            Link = link,
            Message = packet
        }, cancellationToken);

        if (cancelled)
            throw new NotSupportedException("Cancelling manually read packets by protocol support plugins is not supported yet");

        return packet;
    }

    public static async ValueTask SendPacketAsync<T>(this ILink link, CancellationToken cancellationToken) where T : class, IMinecraftPacket, new()
    {
        await link.SendPacketAsync(new T(), cancellationToken);
    }

    public static async ValueTask SendPacketAsync<T>(this ILink link, T packet, CancellationToken cancellationToken) where T : class, IMinecraftPacket
    {
        await link.SendPacketAsync(packet is IClientboundPacket ? Side.Client : Side.Server, packet, cancellationToken);
    }

    public static async ValueTask SendTerminalPacketAsync<T>(this ILink link, CancellationToken cancellationToken) where T : class, IMinecraftPacket, new()
    {
        await link.SendTerminalPacketAsync(new T(), cancellationToken);
    }

    public static async ValueTask SendTerminalPacketAsync<T>(this ILink link, T packet, CancellationToken cancellationToken) where T : class, IMinecraftPacket
    {
        var bundles = link.Player.Context.Services.GetRequiredService<IBundleService>();
        await bundles.WaitBundleCompletionAsync();

        await link.SendPacketAsync(packet is IClientboundPacket ? Side.Client : Side.Server, packet, cancellationToken);
    }

    public static async ValueTask SendPacketAsync<T>(this ILink link, Side side, T packet, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        if (side is Side.Proxy)
            throw new InvalidOperationException("How would I send packet to proxy?");

        var channel = side is Side.Client ? link.PlayerChannel : link.ServerChannel;
        await channel.SendPacketAsync(packet, cancellationToken);

        var events = link.Player.Context.Services.GetRequiredService<IEventService>();
        await events.ThrowAsync(new MessageSentEvent
        {
            Origin = Side.Proxy,
            Direction = side is Side.Client ? Direction.Clientbound : Direction.Serverbound,
            From = Side.Proxy,
            To = side,
            Link = link,
            Message = packet
        }, cancellationToken);
    }
}