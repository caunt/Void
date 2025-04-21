namespace Void.Minecraft.Components.Text.Events.Actions.Click;

public record OpenFile(string File) : IClickEventAction
{
    public string ActionName => "open_file";
}
