using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Players;

namespace Void.Proxy.API.Events.Channels;

public class ChannelCreatedEvent : IEvent
{
    public required IPlayer Initiator { get; init; }
    public required Direction Direction { get; init; }
    public required IMinecraftChannel Channel { get; init; }
}