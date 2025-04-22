using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Commands;

public record ChatCommandEvent(ILink Link, IPlayer Player, string Command, bool IsSigned) : IScopedEventWithResult<bool>
{
    /// <summary>
    ///     <see langword="true" /> if command should not be sent to Server
    /// </summary>
    public bool Result { get; set; }
}
