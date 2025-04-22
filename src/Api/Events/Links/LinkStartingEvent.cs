using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Links;

public record LinkStartingEvent(ILink Link, IPlayer Player) : IScopedEvent;
