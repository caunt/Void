using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Authentication;

public record AuthenticationFinishedEvent(ILink Link, IPlayer Player, AuthenticationSide Side, AuthenticationResult Result) : IScopedEvent;
