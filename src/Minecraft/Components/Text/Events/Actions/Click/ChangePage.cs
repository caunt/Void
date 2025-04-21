namespace Void.Minecraft.Components.Text.Events.Actions.Click;

public record ChangePage(int Page) : IClickEventAction
{
    public string ActionName => "change_page";
}
