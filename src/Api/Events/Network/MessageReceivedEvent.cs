using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.IO.Messages;

namespace Void.Proxy.Api.Events.Network;

public record MessageReceivedEvent(Side Origin, Side From, Side To, Direction Direction, IMinecraftMessage Message, ILink Link) : IEventWithResult<bool>
{
    /// <summary>
    ///     <see langword="true" /> if packet should not be sent
    /// </summary>
    public bool Result { get; set; }
}
