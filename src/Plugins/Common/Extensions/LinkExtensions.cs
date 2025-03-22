using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Links;
using Void.Proxy.API.Links.Extensions;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Messages.Packets;
using Void.Proxy.Plugins.Common.Network.IO.Bundles;

namespace Void.Proxy.Plugins.Common.Extensions;

public static class LinkExtensions
{
    public static async ValueTask<T> ReceivePacketAsync<T>(this ILink link, CancellationToken cancellationToken) where T : class, IMinecraftPacket
    {
        var type = typeof(T);

        if (type == typeof(IMinecraftPacket))
            throw new ArgumentException("You should specify any side-bound packet type");

        var side = type.IsAssignableTo(typeof(IMinecraftClientboundPacket)) ? Side.Server : Side.Client;

        return await link.ReceivePacketAsync<T>(side, cancellationToken);
    }

    public static async ValueTask<T> ReceivePacketAsync<T>(this ILink link, Side side, CancellationToken cancellationToken) where T : IMinecraftPacket
    {
        if (side is Side.Proxy)
            throw new InvalidOperationException("How would I read packet from proxy?");

        var channel = side is Side.Client ? link.PlayerChannel : link.ServerChannel;
        var packet = await channel.ReceivePacketAsync<T>(cancellationToken);

        var events = link.Player.Context.Services.GetRequiredService<IEventService>();
        var direction = side is Side.Client ? Direction.Serverbound : Direction.Clientbound;
        var cancelled = await events.ThrowWithResultAsync(new MessageReceivedEvent(side, side, Side.Proxy, direction, packet, link), cancellationToken);

        if (cancelled)
            throw new NotSupportedException("Cancelling manually read packets by protocol support plugins is not supported yet");

        return packet;
    }

    public static async ValueTask SendTerminalPacketAsync<T>(this ILink link, CancellationToken cancellationToken) where T : class, IMinecraftPacket, new()
    {
        await link.SendTerminalPacketAsync(new T(), cancellationToken);
    }

    public static async ValueTask SendTerminalPacketAsync<T>(this ILink link, T packet, CancellationToken cancellationToken) where T : class, IMinecraftPacket
    {
        var bundles = link.Player.Context.Services.GetRequiredService<IBundleService>();
        await bundles.WaitBundleCompletionAsync();

        await link.SendPacketAsync(packet is IMinecraftClientboundPacket ? Side.Client : Side.Server, packet, cancellationToken);
    }
}