using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Events;

public record PhaseChangedEvent(IPlayer Player, Side Side, INetworkChannel Channel, Phase Phase) : IScopedEvent;
