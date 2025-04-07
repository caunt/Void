using Void.Common.Events;
using Void.Common.Players;

namespace Void.Proxy.Api.Events.Player;

public record PlayerConnectedEvent(IPlayer Player) : IEvent;
