using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Links;

public record LinkStoppedEvent(ILink Link, IPlayer Player, LinkStopReason Reason) : IScopedEvent;
