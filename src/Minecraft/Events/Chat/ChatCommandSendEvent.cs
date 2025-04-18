using Void.Minecraft.Players;
using Void.Proxy.Api.Events;

namespace Void.Minecraft.Events.Chat;

public record ChatCommandSendEvent(IMinecraftPlayer Player, string Command) : IEventWithResult<ChatSendResult>
{
    public ChatSendResult Result { get; set; }
}
