namespace Void.Minecraft.Components.Text.Events.Actions.Click;

/// <summary>Represents a click action that places command text into the client's input field.</summary>
/// <param name="Command">The command text to suggest.</param>
public record SuggestCommand(string Command) : IClickEventAction
{
    /// <summary>Gets the <c>suggest_command</c> click-action identifier.</summary>
    public string ActionName => "suggest_command";
}
