using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Void.Proxy.Api.Events.Channels;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Network.Exceptions;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Players.Extensions;

public static class PlayerExtensions
{
    private static readonly AsyncLock _lock = new();

    extension(IPlayer player)
    {
        public IServer? Server => player.Link?.Server;

        public ILink? Link
        {
            get
            {
                var links = player.GetRequiredService<ILinkService>();

                if (!links.TryGetLink(player, out var link))
                    return null;

                return link;
            }
        }

        public ILink? WeakLink
        {
            get
            {
                var links = player.GetRequiredService<ILinkService>();

                if (!links.TryGetWeakLink(player, out var link))
                    return null;

                return link;
            }
        }

        public bool HasLink
        {
            get
            {
                var links = player.GetRequiredService<ILinkService>();
                return links.HasLink(player);
            }
        }

        public void TrafficPause(Direction direction = Direction.Clientbound | Direction.Serverbound, Operation operation = Operation.Any)
        {
            if (player.Link is not { } link)
            {
                if (direction.HasFlag(Direction.Clientbound) && operation.HasFlag(Operation.Read))
                    throw new InvalidOperationException($"Cannot pause {nameof(Direction.Clientbound)} reading traffic when player is not linked to any server");

                if (direction.HasFlag(Direction.Serverbound) && operation.HasFlag(Operation.Write))
                    throw new InvalidOperationException($"Cannot pause {nameof(Direction.Serverbound)} writing traffic when player is not linked to any server");

                if (player.Context.Channel is { } channel)
                {
                    player.Context.Logger.LogTrace("Pausing player {Player} channel for direction {Direction} and operation {Operation}", player, direction, operation);
                    channel.Pause(operation);
                }

                return;
            }

            if (direction.HasFlag(Direction.Clientbound))
            {
                if (operation.HasFlag(Operation.Read))
                {
                    player.Context.Logger.LogTrace("Pausing link {Link} reading traffic from server for player {Player}", link, player);
                    link.ServerChannel.Pause(operation);
                }

                if (operation.HasFlag(Operation.Write))
                {
                    player.Context.Logger.LogTrace("Pausing link {Link} writing traffic to server for player {Player}", link, player);
                    link.PlayerChannel.Pause(operation);
                }
            }

            if (direction.HasFlag(Direction.Serverbound))
            {
                if (operation.HasFlag(Operation.Read))
                {
                    player.Context.Logger.LogTrace("Pausing link {Link} reading traffic from player for player {Player}", link, player);
                    link.PlayerChannel.Pause(operation);
                }

                if (operation.HasFlag(Operation.Write))
                {
                    player.Context.Logger.LogTrace("Pausing link {Link} writing traffic to player for player {Player}", link, player);
                    link.ServerChannel.Pause(operation);
                }
            }
        }

        public void TrafficContinue(Direction direction = Direction.Clientbound | Direction.Serverbound, Operation operation = Operation.Any)
        {
            if (player.Link is not { } link)
            {
                if (direction.HasFlag(Direction.Clientbound) && operation.HasFlag(Operation.Read))
                    throw new InvalidOperationException($"Cannot continue {nameof(Direction.Clientbound)} reading traffic when player is not linked to any server");

                if (direction.HasFlag(Direction.Serverbound) && operation.HasFlag(Operation.Write))
                    throw new InvalidOperationException($"Cannot continue {nameof(Direction.Serverbound)} writing traffic when player is not linked to any server");

                if (player.Context.Channel is { } channel)
                {
                    player.Context.Logger.LogTrace("Continuing player {Player} channel for direction {Direction} and operation {Operation}", player, direction, operation);
                    channel.Resume(operation);
                }

                return;
            }

            if (direction.HasFlag(Direction.Clientbound))
            {
                if (operation.HasFlag(Operation.Read))
                {
                    player.Context.Logger.LogTrace("Continuing link {Link} reading traffic from server for player {Player}", link, player);
                    link.ServerChannel.Resume(operation);
                }

                if (operation.HasFlag(Operation.Write))
                {
                    player.Context.Logger.LogTrace("Continuing link {Link} writing traffic to server for player {Player}", link, player);
                    link.PlayerChannel.Resume(operation);
                }
            }

            if (direction.HasFlag(Direction.Serverbound))
            {
                if (operation.HasFlag(Operation.Read))
                {
                    player.Context.Logger.LogTrace("Continuing link {Link} reading traffic from player for player {Player}", link, player);
                    link.PlayerChannel.Resume(operation);
                }

                if (operation.HasFlag(Operation.Write))
                {
                    player.Context.Logger.LogTrace("Continuing link {Link} writing traffic to player for player {Player}", link, player);
                    link.ServerChannel.Resume(operation);
                }
            }
        }

        public async ValueTask KickAsync(string text, CancellationToken cancellationToken = default)
        {
            using var disposable = await _lock.LockAsync(cancellationToken);
            await player.GetRequiredService<IPlayerService>().KickPlayerAsync(player, text, cancellationToken);
        }

        public async ValueTask<bool> IsProtocolSupportedAsync(CancellationToken cancellationToken = default)
        {
            var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
            return !channelBuilder.IsFallbackBuilder;
        }

        public async ValueTask<INetworkChannel> BuildServerChannelAsync(IServer server, CancellationToken cancellationToken = default)
        {
            var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
            var channel = await channelBuilder.BuildServerChannelAsync(player, server, cancellationToken);

            var events = player.GetRequiredService<IEventService>();
            await events.ThrowAsync(new ChannelCreatedEvent(player, Side.Server, channel), cancellationToken);
            return channel;
        }

        /// <summary>
        /// Gets the client-side <see cref="INetworkChannel"/> associated with the current player, creating it on first use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If <see cref="global::Void.Proxy.Api.Players.Contexts.IPlayerContext.Channel"/> already contains a channel, this method returns that instance immediately. It does not rebuild the channel and does not publish another <see cref="ChannelCreatedEvent"/>.
        /// </para>
        /// <para>
        /// When no channel has been created yet, the method resolves an <see cref="IChannelBuilderService"/>, runs channel-builder discovery, builds the player-facing channel, stores it in <see cref="global::Void.Proxy.Api.Players.Contexts.IPlayerContext.Channel"/>, and then publishes a <see cref="ChannelCreatedEvent"/> for <see cref="Side.Client"/>.
        /// </para>
        /// <para>
        /// The built-in channel builder consumes the player's initial handshake bytes during discovery and prepends those buffered bytes to the newly created channel before it is returned, so callers still observe the original unread client data through the channel pipeline.
        /// </para>
        /// <para>
        /// This method is the canonical way to obtain a player's inbound channel before reading from it or writing clientbound messages directly through the returned <see cref="INetworkChannel"/>.
        /// </para>
        /// </remarks>
        /// <param name="cancellationToken">
        /// A token that cancels channel-builder discovery, channel construction, or <see cref="ChannelCreatedEvent"/> dispatch before the channel is returned.
        /// </param>
        /// <returns>
        /// A <see cref="ValueTask{TResult}"/> that resolves to the cached or newly created client-side channel. Repeated calls return the same instance until <see cref="global::Void.Proxy.Api.Players.Contexts.IPlayerContext.Channel"/> is replaced.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// <paramref name="cancellationToken"/> was canceled before channel creation or event dispatch completed.
        /// </exception>
        /// <exception cref="EndOfStreamException">
        /// Channel-builder discovery could not read the initial bytes needed to identify a channel implementation for the player connection.
        /// </exception>
        /// <exception cref="StreamClosedException">
        /// The player's scoped services were already disposed while resolving required services, which indicates that the player connection has already been closed.
        /// </exception>
        /// <example>
        /// <code>
        /// var channel = await player.GetChannelAsync(cancellationToken);
        /// await channel.FlushAsync(cancellationToken);
        /// </code>
        /// </example>
        /// <seealso cref="IChannelBuilderService"/>
        /// <seealso cref="ChannelCreatedEvent"/>
        public async ValueTask<INetworkChannel> GetChannelAsync(CancellationToken cancellationToken = default)
        {
            if (player.Context.Channel is not null)
                return player.Context.Channel;

            var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
            var channel = await channelBuilder.BuildPlayerChannelAsync(player, cancellationToken);

            player.Context.Channel = channel;

            var events = player.GetRequiredService<IEventService>();
            await events.ThrowAsync(new ChannelCreatedEvent(player, Side.Client, player.Context.Channel), cancellationToken);

            return player.Context.Channel;
        }

        internal async ValueTask<IChannelBuilderService> GetChannelBuilderAsync(CancellationToken cancellationToken = default)
        {
            var channelBuilder = player.GetRequiredService<IChannelBuilderService>();
            await channelBuilder.SearchChannelBuilderAsync(player, cancellationToken);

            return channelBuilder;
        }

        private TService GetRequiredService<TService>() where TService : notnull
        {
            try
            {
                return player.Context.Services.GetRequiredService<TService>();
            }
            catch (ObjectDisposedException)
            {
                // Player is not online anymore
                throw new StreamClosedException();
            }
        }
    }
}
