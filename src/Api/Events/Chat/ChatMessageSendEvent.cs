using Void.Minecraft.Components.Text;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Chat;

public record ChatMessageSendEvent(IPlayer Player, Component Text) : IEventWithResult<ChatMessageSendResult>
{
    public ChatMessageSendResult Result { get; set; }
}
