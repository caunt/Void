using Void.Proxy.API.Links;

namespace Void.Proxy.API.Events.Authentication;

public record AuthenticationStartingEvent(ILink Link) : IEventWithResult<AuthenticationSide>
{
    public AuthenticationSide Result { get; set; }
}