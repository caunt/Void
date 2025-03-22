using Void.Proxy.API.Links;

namespace Void.Proxy.API.Events.Authentication;

public record AuthenticationFinishedEvent(ILink Link, AuthenticationSide Side, AuthenticationResult Result) : IEvent;