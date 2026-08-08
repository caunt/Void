using Void.Proxy.Api.Events;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;

namespace Void.Minecraft.Events.Chat;

/// <summary>
/// Requests that a command be sent on behalf of a Minecraft player.
/// </summary>
/// <param name="Player">The player for whom the command is sent.</param>
/// <param name="Command">The command text to send.</param>
/// <param name="Origin">The pipeline side that initiated the request.</param>
public record ChatCommandSendEvent(IPlayer Player, string Command, Side Origin) : IScopedEventWithResult<ChatSendResult>
{
    /// <summary>
    /// Gets or sets the outcome reported by the handler that attempted to send the command.
    /// </summary>
    /// <value>The send outcome. Its default value is <see cref="ChatSendResult.NotSupported" />.</value>
    public ChatSendResult Result { get; set; }
}
