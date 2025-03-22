using Void.Proxy.API.Links;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Messages;

namespace Void.Proxy.API.Events.Network;

public record MessageReceivedEvent(Side Origin, Side From, Side To, Direction Direction, IMinecraftMessage Message, ILink Link) : IEventWithResult<bool>
{
    /// <summary>
    ///     <see langword="true" /> if packet should not be sent
    /// </summary>
    public bool Result { get; set; }
}