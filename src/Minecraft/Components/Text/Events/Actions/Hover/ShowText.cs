namespace Void.Minecraft.Components.Text.Events.Actions.Hover;

public record ShowText(Component Value) : IHoverEventAction
{
    public string ActionName => "show_text";
}
