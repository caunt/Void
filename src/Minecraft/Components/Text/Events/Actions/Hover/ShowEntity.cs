using Void.Minecraft.Profiles;

namespace Void.Minecraft.Components.Text.Events.Actions.Hover;

public record ShowEntity(Uuid Id, string? Type = null, Component? Name = null) : IHoverEventAction
{
    public string ActionName => "show_entity";
}
