using Void.Common.Events;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Players;

namespace Void.Minecraft.Events.Chat;

public record ChatMessageSendEvent(IMinecraftPlayer Player, Component Text) : IEventWithResult<ChatMessageSendResult>
{
    public ChatMessageSendResult Result { get; set; }
}
