namespace Void.Minecraft.Components.Text.Events.Actions.Hover;

public record ShowEntity(Component Name, string Type, Uuid Id) : IHoverEventAction
{
    public string ActionName => "show_entity";
}
