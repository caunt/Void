namespace Void.Minecraft.Components.Text.Events.Actions.Click;

/// <summary>Represents a click action that copies text to the client's clipboard.</summary>
/// <param name="Value">The text to copy.</param>
public record CopyToClipboard(string Value) : IClickEventAction
{
    /// <summary>Gets the <c>copy_to_clipboard</c> click-action identifier.</summary>
    public string ActionName => "copy_to_clipboard";
}
