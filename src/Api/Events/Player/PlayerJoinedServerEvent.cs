using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Player;

/// <summary>
/// Event that occurs when a player joins a server and both sides are in Play phase.
/// </summary>
/// <param name="Player">The player who has joined the server. Cannot be null.</param>
/// <param name="Server">The server that the player has joined. Cannot be null.</param>
/// <param name="Link">The link associated with the player's connection to the server. Cannot be null.</param>
public record PlayerJoinedServerEvent(IPlayer Player, IServer Server, ILink Link) : IScopedEvent;
