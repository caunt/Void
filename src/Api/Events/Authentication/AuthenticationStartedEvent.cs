using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Authentication;

public record AuthenticationStartedEvent(ILink Link, IPlayer Player, AuthenticationSide Side) : IScopedEventWithResult<AuthenticationResult>
{
    public AuthenticationResult Result { get; set; }
}
