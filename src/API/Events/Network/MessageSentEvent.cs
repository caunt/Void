using Void.Proxy.API.Links;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Messages;

namespace Void.Proxy.API.Events.Network;

public class MessageSentEvent : IEvent
{
    public required Side Origin { get; init; }
    public required Side From { get; init; }
    public required Side To { get; init; }
    public required Direction Direction { get; init; }
    public required IMinecraftMessage Message { get; init; }
    public required ILink Link { get; init; }
}