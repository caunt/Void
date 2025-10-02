using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network.Channels.Extensions;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Network.Registries;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;

namespace Void.Minecraft.Links.Extensions;

public static class LinkExtensions
{
    public static async ValueTask SendPacketAsync<T>(this ILink link, CancellationToken cancellationToken) where T : class, IMinecraftMessage, new()
    {
        await link.SendPacketAsync(new T(), cancellationToken);
    }

    public static async ValueTask SendPacketAsync<T>(this ILink link, T packet, CancellationToken cancellationToken) where T : class, IMinecraftMessage
    {
        var side = packet switch
        {
            IMinecraftClientboundPacket => Side.Client,
            IMinecraftServerboundPacket => Side.Server,
            _ => throw new InvalidOperationException($"Packet does not implement {nameof(IMinecraftClientboundPacket)} nor {nameof(IMinecraftServerboundPacket)} interface")
        };

        await link.SendPacketAsync(side, packet, cancellationToken);
    }

    public static async ValueTask SendPacketAsync<T>(this ILink link, Side side, T packet, CancellationToken cancellationToken) where T : IMinecraftMessage
    {
        if (side is Side.Proxy)
            throw new InvalidOperationException("What do you mean by sending packet to proxy side?");

        var channel = side is Side.Client ? link.PlayerChannel : link.ServerChannel;
        await channel.SendPacketAsync(packet, cancellationToken);

        var events = link.Player.Context.Services.GetRequiredService<IEventService>();
        var direction = side is Side.Client ? Direction.Clientbound : Direction.Serverbound;
        await events.ThrowAsync(new MessageSentEvent(Side.Proxy, Side.Proxy, side, direction, packet, link, link.Player), cancellationToken);
    }
}
