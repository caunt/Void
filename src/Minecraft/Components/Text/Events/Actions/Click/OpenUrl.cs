namespace Void.Minecraft.Components.Text.Events.Actions.Click;

public record OpenUrl(string Url) : IClickEventAction
{
    public string ActionName => "open_url";
}
