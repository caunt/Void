using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Authentication;

public record AuthenticationStartingEvent(ILink Link, IPlayer Player) : IScopedEventWithResult<AuthenticationSide>
{
    public AuthenticationSide Result { get; set; }
}
