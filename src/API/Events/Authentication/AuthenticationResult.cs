namespace Void.Proxy.API.Events.Authentication;

public enum AuthenticationResult
{
    NoResult,
    NotAuthenticatedPlayer,
    NotAuthenticatedServer,
    AlreadyAuthenticated,
    Authenticated
}
