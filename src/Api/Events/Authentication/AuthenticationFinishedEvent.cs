using Void.Common.Events;
using Void.Proxy.Api.Links;

namespace Void.Proxy.Api.Events.Authentication;

public record AuthenticationFinishedEvent(ILink Link, AuthenticationSide Side, AuthenticationResult Result) : IEvent;
