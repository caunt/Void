using Void.Minecraft.Network;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Minecraft;

public record PhaseChangedEvent(IPlayer Player, Side Side, IMinecraftChannel Channel, Phase Phase) : IEvent;
