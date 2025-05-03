using Void.Minecraft.Components.Text;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Events;

public record MinecraftPlayerKickEvent(IPlayer Player, Component? Reason = null) : PlayerKickEvent(Player, Reason?.SerializeLegacy());
