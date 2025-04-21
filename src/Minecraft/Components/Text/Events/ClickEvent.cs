using Void.Minecraft.Components.Text.Events.Actions;

namespace Void.Minecraft.Components.Text.Events;

public record ClickEvent(IClickEventAction Content) : IEvent
{
    public string ActionName => Content.ActionName;
}
