using Void.Proxy.API.Players;

namespace Void.Proxy.API.Events.Player;

public record PlayerKickEvent(IPlayer Player, string? Reason = null) : IEventWithResult<bool>
{
    /// <summary>
    ///     <see langword="true" /> if kick was made; otherwise, <see langword="false" />.
    /// </summary>
    public bool Result { get; set; }
}