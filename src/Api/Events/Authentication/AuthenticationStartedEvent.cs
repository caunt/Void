using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Authentication;

public record AuthenticationStartedEvent(ILink Link, IPlayer Player, AuthenticationSide Side) : IScopedEventWithResult<AuthenticationResult>
{
    public AuthenticationResult? Result { get; set; }
}

public record AuthenticationResult(bool IsAuthenticated, string? Message = null)
{
    public static AuthenticationResult Authenticated => new(true, "Authenticated");
    public static AuthenticationResult AlreadyAuthenticated => new(true, "Already Authenticated");
    public static AuthenticationResult NotAuthenticatedPlayer => new(false, "Not Authenticated Player");
    public static AuthenticationResult NotAuthenticatedServer => new(false, "Not Authenticated Server");
    public static AuthenticationResult NoResult => new(false, "No Result");
}
