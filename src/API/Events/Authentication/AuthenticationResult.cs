namespace Void.Proxy.Api.Events.Authentication;

public enum AuthenticationResult
{
    NoResult,
    NotAuthenticatedPlayer,
    NotAuthenticatedServer,
    AlreadyAuthenticated,
    Authenticated
}
