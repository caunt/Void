namespace Void.Minecraft.Components.Text.Events.Actions.Click;

/// <summary>Represents a click action that submits a command through the client.</summary>
/// <param name="Command">The command text, including any leading slash expected by the client.</param>
public record RunCommand(string Command) : IClickEventAction
{
    /// <summary>Gets the <c>run_command</c> click-action identifier.</summary>
    public string ActionName => "run_command";
}
