using Void.Minecraft.Components.Text.Events.Actions;

namespace Void.Minecraft.Components.Text.Events;

/// <summary>Wraps a click action for a text component.</summary>
/// <param name="Content">The click-action payload.</param>
public record ClickEvent(IClickEventAction Content) : IEvent
{
    /// <summary>Gets the action identifier supplied by <see cref="Content"/>.</summary>
    public string ActionName => Content.ActionName;
}
