namespace Void.Minecraft.Components.Text.Events.Actions.Hover;

public record ShowItem(string Id, int Count, byte[] ItemComponents) : IHoverEventAction
{
    public string ActionName => "show_item";
}
