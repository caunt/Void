using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Channels;

public record ChannelCreatedEvent(IPlayer Player, Side Side, IMinecraftChannel Channel) : IEvent;