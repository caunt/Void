namespace Void.Minecraft.Events.Chat;

/// <summary>
/// Describes the outcome of a request to send Minecraft chat or a command.
/// </summary>
public enum ChatSendResult
{
    /// <summary>
    /// No installed protocol handler supports sending the requested content.
    /// </summary>
    NotSupported,

    /// <summary>
    /// The player is not in the play protocol phase required for sending chat.
    /// </summary>
    NotPlaying,

    /// <summary>
    /// The content was sent successfully.
    /// </summary>
    Success
}
