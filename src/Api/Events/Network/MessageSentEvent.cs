using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Network;

/// <param name="From">The side that immediately sent the message to <paramref name="To" />. For a forwarded message, this can differ from <paramref name="Origin" />.</param>
public record MessageSentEvent(Side Origin, Side From, Side To, Direction Direction, INetworkMessage Message, ILink Link, IPlayer Player) : IScopedEvent;
