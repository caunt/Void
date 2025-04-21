namespace Void.Minecraft.Components.Text.Events.Actions.Click;

public record CopyToClipboard(string Value) : IClickEventAction
{
    public string ActionName => "copy_to_clipboard";
}
