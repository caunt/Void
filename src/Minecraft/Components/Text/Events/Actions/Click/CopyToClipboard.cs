namespace Void.Minecraft.Components.Text.Events.Actions.Click;

public record CopyToClipboard : IClickEventAction
{
    public string ActionName => "copy_to_clipboard";
}
