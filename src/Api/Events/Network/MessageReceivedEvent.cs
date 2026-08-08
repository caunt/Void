using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Network;

/// <summary>
/// Represents a network message after it has been read and before it is forwarded to its next processing stage.
/// </summary>
/// <param name="Origin">The side from which the message originally entered the proxy pipeline.</param>
/// <param name="From">The side that produced the message for the current processing stage.</param>
/// <param name="To">The side receiving the message in the current processing stage.</param>
/// <param name="Direction">The protocol direction in which the message is traveling.</param>
/// <param name="Message">The decoded or raw network message being processed.</param>
/// <param name="Link">The link carrying the message.</param>
/// <param name="Player">The player associated with the message.</param>
public record MessageReceivedEvent(Side Origin, Side From, Side To, Direction Direction, INetworkMessage Message, ILink Link, IPlayer Player) : IScopedEventWithResult<bool>
{
    /// <summary>
    /// Gets or sets whether forwarding the message is canceled.
    /// </summary>
    /// <value><see langword="true" /> to suppress forwarding; otherwise, <see langword="false" />.</value>
    public bool Result { get; set; }

    /// <summary>
    /// Cancels forwarding if it has not already been canceled.
    /// </summary>
    /// <returns><see langword="true" /> when forwarding was already canceled; otherwise, <see langword="false" /> after setting <see cref="Result" />.</returns>
    public bool Cancel()
    {
        if (Result)
            return true;

        Result = true;
        return false;
    }
}
