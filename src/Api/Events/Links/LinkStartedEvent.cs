using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Links;

public record LinkStartedEvent(ILink Link, IPlayer Player) : IScopedEvent;
