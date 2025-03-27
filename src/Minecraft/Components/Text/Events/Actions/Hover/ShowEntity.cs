using Void.Minecraft.Profiles;

namespace Void.Minecraft.Components.Text.Events.Actions.Hover;

public record ShowEntity(string Type, Uuid Id, Component? Name = null) : IHoverEventAction
{
    public string ActionName => "show_entity";
}
