using Void.Proxy.Api.Links;

namespace Void.Proxy.Api.Events.Authentication;

public record AuthenticationStartingEvent(ILink Link) : IEventWithResult<AuthenticationSide>
{
    public AuthenticationSide Result { get; set; }
}