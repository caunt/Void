using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network.Channels.Extensions;
using Void.Minecraft.Network.Messages;
using Void.Minecraft.Network.Messages.Packets;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;

namespace Void.Minecraft.Links.Extensions;

/// <summary>
/// Provides packet-oriented send operations for player-to-server links.
/// </summary>
public static class LinkExtensions
{
    /// <summary>
    /// Constructs and sends a parameterless Minecraft message to the side implied by its marker interface.
    /// </summary>
    /// <typeparam name="T">The message type to construct and send.</typeparam>
    /// <param name="link">The link through which the message is sent.</param>
    /// <param name="cancellationToken">A token used to cancel sending and event publication.</param>
    /// <returns>A task that completes after the message is sent and the sent event is processed.</returns>
    /// <exception cref="InvalidOperationException"><typeparamref name="T" /> is neither clientbound nor serverbound.</exception>
    public static async ValueTask SendPacketAsync<T>(this ILink link, CancellationToken cancellationToken) where T : class, IMinecraftMessage, new()
    {
        await link.SendPacketAsync(new T(), cancellationToken);
    }

    /// <summary>
    /// Sends a Minecraft message to the side implied by its marker interface.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="link">The link through which the message is sent.</param>
    /// <param name="packet">The message instance to send.</param>
    /// <param name="cancellationToken">A token used to cancel sending and event publication.</param>
    /// <returns>A task that completes after the message is sent and the sent event is processed.</returns>
    /// <exception cref="InvalidOperationException"><paramref name="packet" /> is neither clientbound nor serverbound.</exception>
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

    /// <summary>
    /// Sends a Minecraft message to an explicitly selected link side and publishes a corresponding message-sent event.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="link">The link through which the message is sent.</param>
    /// <param name="side">The destination side. <see cref="Side.Client" /> uses the player channel and <see cref="Side.Server" /> uses the server channel.</param>
    /// <param name="packet">The message instance to send.</param>
    /// <param name="cancellationToken">A token used to cancel sending and event publication.</param>
    /// <returns>A task that completes after the message is sent and the sent event is processed.</returns>
    /// <exception cref="InvalidOperationException"><paramref name="side" /> is <see cref="Side.Proxy" />.</exception>
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
