using Void.Common.Network;
using Void.Common.Network.Channels;
using Void.Minecraft.Network;
using Void.Minecraft.Players;
using Void.Proxy.Api.Events;

namespace Void.Minecraft.Events;

public record PhaseChangedEvent(IMinecraftPlayer Player, Side Side, INetworkChannel Channel, Phase Phase) : IEvent;
