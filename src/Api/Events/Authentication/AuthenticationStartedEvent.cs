using Void.Proxy.Api.Links;

namespace Void.Proxy.Api.Events.Authentication;

public record AuthenticationStartedEvent(ILink Link, AuthenticationSide Side) : IEventWithResult<AuthenticationResult>
{
    public AuthenticationResult Result { get; set; }
}
