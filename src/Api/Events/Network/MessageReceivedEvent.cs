using Void.Common;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;

namespace Void.Proxy.Api.Events.Network;

public record MessageReceivedEvent(Side Origin, Side From, Side To, Direction Direction, INetworkMessage Message, ILink Link) : IEventWithResult<bool>
{
    /// <summary>
    ///     <see langword="true" /> if packet should not be sent
    /// </summary>
    public bool Result { get; set; }
}
