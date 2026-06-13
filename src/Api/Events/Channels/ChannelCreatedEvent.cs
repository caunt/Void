using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Channels;

/// <param name="Channel">The <see cref="INetworkChannel"/> that was created for the event's <see cref="Side"/>.</param>
public record ChannelCreatedEvent(IPlayer Player, Side Side, INetworkChannel Channel) : IScopedEvent;
