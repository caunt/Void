using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Authentication;

public record AuthenticationStartedEvent(ILink Link, IPlayer Player, AuthenticationSide Side) : IScopedEventWithResult<AuthenticationResult>
{
    public AuthenticationResult? Result { get; set; }
}

public record AuthenticationResult(bool IsAuthenticated, string? Message = null)
{
    /// <summary>
    /// Gets a successful authentication result with a default human-readable message.
    /// </summary>
    /// <value>
    /// A new <see cref="AuthenticationResult"/> instance where <see cref="AuthenticationResult.IsAuthenticated"/> is <see langword="true"/>
    /// and <see cref="AuthenticationResult.Message"/> is <c>"Authenticated"</c>.
    /// </value>
    /// <remarks>
    /// <para>
    /// Each access creates a new record instance instead of returning a cached object.
    /// </para>
    /// <para>
    /// Use this value for successful authentication branches that should compare by record value semantics
    /// (for example, checks against <see cref="NoResult"/> or other predefined outcomes).
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = AuthenticationResult.Authenticated;
    /// if (result.IsAuthenticated)
    /// {
    ///     // Continue login flow.
    /// }
    /// </code>
    /// </example>
    /// <see cref="IsAuthenticated" />
    /// <seealso cref="AlreadyAuthenticated" />
    /// <seealso cref="NoResult" />
    public static AuthenticationResult Authenticated => new(true, "Authenticated");
    public static AuthenticationResult AlreadyAuthenticated => new(true, "Already Authenticated");
    public static AuthenticationResult NotAuthenticatedPlayer => new(false, "Not Authenticated Player");
    public static AuthenticationResult NotAuthenticatedServer => new(false, "Not Authenticated Server");
    public static AuthenticationResult NoResult => new(false, "No Result");
}
