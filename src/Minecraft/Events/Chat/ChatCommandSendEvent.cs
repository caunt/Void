using Void.Minecraft.Players;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;

namespace Void.Minecraft.Events.Chat;

public record ChatCommandSendEvent(IMinecraftPlayer Player, string Command, Side Origin) : IEventWithResult<ChatSendResult>
{
    public ChatSendResult Result { get; set; }
}
