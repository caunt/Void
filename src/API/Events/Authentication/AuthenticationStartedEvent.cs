using Void.Proxy.API.Links;

namespace Void.Proxy.API.Events.Authentication;

public record AuthenticationStartedEvent(ILink Link, AuthenticationSide Side) : IEventWithResult<AuthenticationResult>
{
    public AuthenticationResult Result { get; set; }
}