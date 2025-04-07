using Void.Common.Events;
using Void.Common.Players;

namespace Void.Proxy.Api.Events.Player;

public record PlayerKickEvent(IPlayer Player, string? Text = null) : IEventWithResult<bool>
{
    /// <summary>
    ///     <see langword="true" /> if kick was made; otherwise, <see langword="false" />.
    /// </summary>
    public bool Result { get; set; }
}
