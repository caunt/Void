using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Void.Proxy.Api.Events.Channels;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Players.Extensions;

public static class PlayerExtensions
{
    private static readonly AsyncLock _lock = new();

    public static IServer? GetServer(this IPlayer player)
    {
        if (!player.TryGetLink(out var link))
            return null;

        return link.Server;
    }

    public static bool TryGetLink(this IPlayer player, [MaybeNullWhen(false)] out ILink link)
    {
        var links = player.Context.Services.GetRequiredService<ILinkService>();
        return links.TryGetLink(player, out link);
    }

    public static ILink GetLink(this IPlayer player)
    {
        if (!player.TryGetLink(out var link))
            throw new InvalidOperationException("Player is not linked to any server");

        return link;
    }

    public static void TrafficPause(this IPlayer player, Direction direction = Direction.Clientbound | Direction.Serverbound, Operation operation = Operation.Any)
    {
        if (!player.TryGetLink(out var link))
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

    public static void TrafficContinue(this IPlayer player, Direction direction = Direction.Clientbound | Direction.Serverbound, Operation operation = Operation.Any)
    {
        if (!player.TryGetLink(out var link))
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

    public static async ValueTask KickAsync(this IPlayer player, string text, CancellationToken cancellationToken = default)
    {
        // Synchronize in case two threads try to kick the same player at the same time
        using var _ = await _lock.LockAsync(cancellationToken);

        // Might be called twice, so just handle first one
        if (player.Context.IsDisposed)
            return;

        var players = player.Context.Services.GetRequiredService<IPlayerService>();
        await players.KickPlayerAsync(player, text, cancellationToken);

        if (!player.Context.IsDisposed)
            throw new Exception("Player context should be disposed after kick");
    }

    public static async ValueTask<bool> IsProtocolSupportedAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
        return !channelBuilder.IsFallbackBuilder;
    }

    public static async ValueTask<INetworkChannel> BuildServerChannelAsync(this IPlayer player, IServer server, CancellationToken cancellationToken = default)
    {
        var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
        var channel = await channelBuilder.BuildServerChannelAsync(player, server, cancellationToken);

        var events = player.Context.Services.GetRequiredService<IEventService>();
        await events.ThrowAsync(new ChannelCreatedEvent(player, Network.Side.Server, channel), cancellationToken);

        return channel;
    }

    public static async ValueTask<INetworkChannel> GetChannelAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        if (player.Context.Channel is not null)
            return player.Context.Channel;

        var channelBuilder = await player.GetChannelBuilderAsync(cancellationToken);
        var channel = await channelBuilder.BuildPlayerChannelAsync(player, cancellationToken);

        player.Context.Channel = channel;

        var events = player.Context.Services.GetRequiredService<IEventService>();
        await events.ThrowAsync(new ChannelCreatedEvent(player, Network.Side.Client, player.Context.Channel), cancellationToken);

        return player.Context.Channel;
    }

    internal static async ValueTask<IChannelBuilderService> GetChannelBuilderAsync(this IPlayer player, CancellationToken cancellationToken = default)
    {
        var channelBuilder = player.Context.Services.GetRequiredService<IChannelBuilderService>();
        await channelBuilder.SearchChannelBuilderAsync(player, cancellationToken);

        return channelBuilder;
    }
}
