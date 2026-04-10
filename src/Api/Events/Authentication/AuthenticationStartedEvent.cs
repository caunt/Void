using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Authentication;

/// <summary>
/// Raised when the proxy is about to authenticate a connection. The authentication side indicates
/// whether the handshake is with the connecting player or with the backend server.
/// Handlers set <see cref="Result" /> to report the outcome of their authentication logic.
/// </summary>
/// <param name="Link">The link over which authentication is taking place.</param>
/// <param name="Player">The player whose connection is being authenticated.</param>
/// <param name="Side">
/// Indicates which side is being authenticated: <see cref="AuthenticationSide.Proxy" /> when the
/// proxy is authenticating the player, or <see cref="AuthenticationSide.Server" /> when the proxy
/// is authenticating with a backend server.
/// </param>
public record AuthenticationStartedEvent(ILink Link, IPlayer Player, AuthenticationSide Side) : IScopedEventWithResult<AuthenticationResult>
{
    /// <summary>
    /// Gets or sets the result of the authentication attempt. Set by handlers to indicate
    /// whether authentication succeeded and to provide an optional message.
    /// </summary>
    public AuthenticationResult? Result { get; set; }
}

/// <summary>
/// Represents the outcome of an authentication attempt, including a success flag and an
/// optional human-readable message. Use the static members for common well-known results.
/// </summary>
/// <param name="IsAuthenticated"><see langword="true" /> if authentication succeeded; otherwise, <see langword="false" />.</param>
/// <param name="Message">An optional message describing the authentication outcome, or <see langword="null" /> if no message applies.</param>
public record AuthenticationResult(bool IsAuthenticated, string? Message = null)
{
    /// <summary>
    /// Gets a result indicating successful authentication.
    /// </summary>
    public static AuthenticationResult Authenticated => new(true, "Authenticated");

    /// <summary>
    /// Gets a result indicating the player was already authenticated on this proxy,
    /// so the authentication step was skipped.
    /// </summary>
    public static AuthenticationResult AlreadyAuthenticated => new(true, "Already Authenticated");

    /// <summary>
    /// Gets a result indicating that authentication failed because the player could not be verified.
    /// </summary>
    public static AuthenticationResult NotAuthenticatedPlayer => new(false, "Not Authenticated Player");

    /// <summary>
    /// Gets a result indicating that authentication failed because the backend server rejected the connection.
    /// </summary>
    public static AuthenticationResult NotAuthenticatedServer => new(false, "Not Authenticated Server");

    /// <summary>
    /// Gets a result indicating that no handler produced an authentication outcome.
    /// </summary>
    public static AuthenticationResult NoResult => new(false, "No Result");
}
