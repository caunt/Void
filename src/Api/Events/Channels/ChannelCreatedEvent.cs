using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Channels;

/// <summary>
/// Represents an event published after a network channel has been created for a player connection.
/// </summary>
/// <remarks>
/// The event is raised after the channel instance is built and before the creating API returns it to the caller. A <see cref="Side.Client"/> value identifies the player-facing channel stored on the player's context, while <see cref="Side.Server"/> identifies a backend server channel created for the same player.
/// </remarks>
/// <param name="Player">The player whose connection owns or initiated the created channel.</param>
/// <param name="Side">The connection side represented by <paramref name="Channel"/>.</param>
/// <param name="Channel">The created channel instance that handlers can inspect or configure.</param>
public record ChannelCreatedEvent(IPlayer Player, Side Side, INetworkChannel Channel) : IScopedEvent;
