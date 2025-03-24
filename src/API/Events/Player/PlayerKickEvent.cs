using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Player;

public record PlayerKickEvent(IPlayer Player, string? Reason = null) : IEventWithResult<bool>
{
    /// <summary>
    ///     <see langword="true" /> if kick was made; otherwise, <see langword="false" />.
    /// </summary>
    public bool Result { get; set; }
}