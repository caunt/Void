using Void.Proxy.API.Links;

namespace Void.Proxy.API.Events.Authentication;

public class AuthenticationFailed : IEvent
{
    public required ILink Link { get; init; }
    public required AuthenticationSide Side { get; init; }
}