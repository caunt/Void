using Void.Common.Events;
using Void.Common.Network;
using Void.Common.Network.Channels;
using Void.Common.Players;

namespace Void.Proxy.Api.Events.Channels;

public record ChannelCreatedEvent(IPlayer Player, Side Side, INetworkChannel Channel) : IEvent;
