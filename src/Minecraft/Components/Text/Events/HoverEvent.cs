using Void.Minecraft.Components.Text.Events.Actions;

namespace Void.Minecraft.Components.Text.Events;

public record HoverEvent(IHoverEventAction Content) : IEvent
{
    public string ActionName => Content.ActionName;
}
