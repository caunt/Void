using Void.Proxy.API.Links;

namespace Void.Proxy.API.Events.Authentication;

public class AuthenticationStartingEvent : IEventWithResult<AuthenticationSide>
{
    public required ILink Link { get; init; }
    public AuthenticationSide Result { get; set; } = AuthenticationSide.Proxy;
}