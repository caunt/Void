using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Links;

public record LinkStoppingEvent(ILink Link, IPlayer Player, LinkStopReason Reason) : IScopedEvent;
