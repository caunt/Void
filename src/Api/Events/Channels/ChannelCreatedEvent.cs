using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Channels;

public record ChannelCreatedEvent(IPlayer Player, Side Side, INetworkChannel Channel) : IScopedEvent;
