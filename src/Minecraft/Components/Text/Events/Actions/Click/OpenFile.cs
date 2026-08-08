namespace Void.Minecraft.Components.Text.Events.Actions.Click;

/// <summary>Represents a click action that asks the client to open a local file.</summary>
/// <param name="File">The local file path supplied to the client.</param>
public record OpenFile(string File) : IClickEventAction
{
    /// <summary>Gets the <c>open_file</c> click-action identifier.</summary>
    public string ActionName => "open_file";
}
