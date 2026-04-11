using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Authentication;

public record AuthenticationStartedEvent(ILink Link, IPlayer Player, AuthenticationSide Side) : IScopedEventWithResult<AuthenticationResult>
{
    /// <summary>
    /// Gets or sets the authentication decision produced by listeners for this event.
    /// </summary>
    /// <value>
    /// The latest <see cref="AuthenticationResult"/> assigned by a listener, or <see langword="null"/> when no listener has decided yet.
    /// </value>
    /// <remarks>
    /// <para>
    /// The event dispatcher returns this property from <see cref="Void.Proxy.Api.Events.Services.IEventService.ThrowWithResultAsync{TResult}(IEventWithResult{TResult}, CancellationToken)"/>.
    /// </para>
    /// <para>
    /// Callers commonly coalesce <see langword="null"/> to <see cref="AuthenticationResult.NoResult"/> to represent an unresolved state.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = await events.ThrowWithResultAsync(authenticationStartedEvent, cancellationToken)
    ///     ?? AuthenticationResult.NoResult;
    /// </code>
    /// </example>
    /// <see cref="AuthenticationResult"/>
    /// <seealso cref="AuthenticationResult.NoResult"/>
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

    /// <summary>
    /// Gets a sentinel authentication result that indicates no final outcome has been produced yet.
    /// </summary>
    /// <value>
    /// A new <see cref="AuthenticationResult"/> instance where <see cref="IsAuthenticated"/> is <see langword="false"/>
    /// and <see cref="Message"/> is <c>"No Result"</c>.
    /// </value>
    /// <remarks>
    /// <para>
    /// This value is used as a control signal while processing server login packets: handlers return it to continue waiting for more packets.
    /// </para>
    /// <para>
    /// Equality checks against <see cref="NoResult"/> are value-based because <see cref="AuthenticationResult"/> is a record, even though each access creates a new instance.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// if (result == AuthenticationResult.NoResult)
    ///     continue;
    /// </code>
    /// </example>
    /// <see cref="AuthenticationStartedEvent.Result"/>
    /// <seealso cref="Authenticated"/>
    public static AuthenticationResult NoResult => new(false, "No Result");
}
