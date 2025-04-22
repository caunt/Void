using Void.Minecraft.Players;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Events.Chat;

public record ChatCommandSendEvent(IMinecraftPlayer Player, string Command, Side Origin) : IScopedEventWithResult<ChatSendResult>
{
    public ChatSendResult Result { get; set; }
    IPlayer IScopedEvent.Player => Player;
}
