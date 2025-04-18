using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Messages;

namespace Void.Proxy.Api.Events.Network;

public record MessageReceivedEvent(Side Origin, Side From, Side To, Direction Direction, INetworkMessage Message, ILink Link) : IEventWithResult<bool>
{
    /// <summary>
    ///     <see langword="true" /> if packet should not be sent
    /// </summary>
    public bool Result { get; set; }

    /// <summary>
    /// Cancels an operation if it hasn't been canceled already. It sets the Result to true upon cancellation.
    /// </summary>
    /// <returns>Returns true if the operation was already canceled; otherwise, returns false.</returns>
    public bool Cancel()
    {
        if (Result)
            return true;

        Result = true;
        return false;
    }
}
