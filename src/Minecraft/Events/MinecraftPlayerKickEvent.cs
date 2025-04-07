using Void.Minecraft.Components.Text;
using Void.Minecraft.Players;
using Void.Proxy.Api.Events.Player;

namespace Void.Minecraft.Events;

public record MinecraftPlayerKickEvent(IMinecraftPlayer MinecraftPlayer, Component? Reason = null) : PlayerKickEvent(MinecraftPlayer, Reason?.SerializeLegacy());
