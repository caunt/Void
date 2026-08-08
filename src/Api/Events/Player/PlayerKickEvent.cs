using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Player;

/// <summary>
/// Requests that a player be disconnected from the proxy.
/// </summary>
/// <param name="Player">The player to disconnect.</param>
/// <param name="Text">The optional disconnect message; <see langword="null" /> uses the implementation's default message.</param>
public record PlayerKickEvent(IPlayer Player, string? Text = null) : IScopedEventWithResult<bool>
{
    /// <summary>
    ///     <see langword="true" /> if kick was made; otherwise, <see langword="false" />.
    /// </summary>
    public bool Result { get; set; }
}
