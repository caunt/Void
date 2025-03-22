using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Players;

namespace Void.Proxy.API.Events.Minecraft;

public record PhaseChangedEvent(IPlayer Player, Side Side, IMinecraftChannel Channel, Phase Phase) : IEvent;