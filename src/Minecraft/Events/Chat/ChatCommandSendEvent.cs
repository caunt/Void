using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Events.Chat;

public record ChatCommandSendEvent(IPlayer Player, string Command, Side Origin) : IScopedEventWithResult<ChatSendResult>
{
    public ChatSendResult Result { get; set; }
}
