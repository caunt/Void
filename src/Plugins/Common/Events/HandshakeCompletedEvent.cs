using Void.Proxy.Api.Events;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Plugins.Common.Events;

public record HandshakeCompletedEvent(IPlayer Player, ILink Link, Side Side, string ServerAddress, int NextState) : IScopedEvent;
