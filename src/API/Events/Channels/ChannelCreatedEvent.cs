using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Players;

namespace Void.Proxy.API.Events.Channels;

public record ChannelCreatedEvent(IPlayer Player, Side Side, IMinecraftChannel Channel) : IEvent;