using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Network;

/// <summary>
/// Represents a network message after it has been forwarded through a processing stage.
/// </summary>
/// <param name="Origin">The side from which the message originally entered the proxy pipeline.</param>
/// <param name="From">The side that sent the message in the current processing stage.</param>
/// <param name="To">The side that received the message in the current processing stage.</param>
/// <param name="Direction">The protocol direction in which the message traveled.</param>
/// <param name="Message">The decoded or raw network message that was sent.</param>
/// <param name="Link">The link carrying the message.</param>
/// <param name="Player">The player associated with the message.</param>
public record MessageSentEvent(Side Origin, Side From, Side To, Direction Direction, INetworkMessage Message, ILink Link, IPlayer Player) : IScopedEvent;
