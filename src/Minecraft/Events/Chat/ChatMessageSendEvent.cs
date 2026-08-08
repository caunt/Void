using Void.Minecraft.Components.Text;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Events.Chat;

/// <summary>
/// Requests that a text component be sent as chat on behalf of a Minecraft player.
/// </summary>
/// <param name="Player">The player for whom the message is sent.</param>
/// <param name="Text">The component to send.</param>
/// <param name="Origin">The pipeline side that initiated the request.</param>
public record ChatMessageSendEvent(IPlayer Player, Component Text, Side Origin) : IScopedEventWithResult<ChatSendResult>
{
    /// <summary>
    /// Gets or sets the outcome reported by the handler that attempted to send the message.
    /// </summary>
    /// <value>The send outcome. Its default value is <see cref="ChatSendResult.NotSupported" />.</value>
    public ChatSendResult Result { get; set; }
}
