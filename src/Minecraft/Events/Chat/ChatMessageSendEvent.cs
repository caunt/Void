using Void.Minecraft.Components.Text;
using Void.Minecraft.Players;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;

namespace Void.Minecraft.Events.Chat;

public record ChatMessageSendEvent(IMinecraftPlayer Player, Component Text, Side Origin) : IEventWithResult<ChatSendResult>
{
    public ChatSendResult Result { get; set; }
}
