using Void.Minecraft.Network;
using Void.Minecraft.Players;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;

namespace Void.Minecraft.Events;

public record PhaseChangedEvent(IMinecraftPlayer Player, Side Side, INetworkChannel Channel, Phase Phase) : IEvent;
