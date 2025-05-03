using Void.Minecraft.Components.Text;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Events.Chat;

public record ChatMessageSendEvent(IPlayer Player, Component Text, Side Origin) : IScopedEventWithResult<ChatSendResult>
{
    public ChatSendResult Result { get; set; }
}
