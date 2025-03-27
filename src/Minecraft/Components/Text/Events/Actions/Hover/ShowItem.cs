namespace Void.Minecraft.Components.Text.Events.Actions.Hover;

public record ShowItem(string Id, int? Count = 0, byte[]? ItemComponents = null) : IHoverEventAction
{
    public string ActionName => "show_item";
}
