using Void.Minecraft.Components.Text.Events.Actions;

namespace Void.Minecraft.Components.Text.Events;

/// <summary>Wraps a hover action for a text component.</summary>
/// <param name="Content">The hover-action payload.</param>
public record HoverEvent(IHoverEventAction Content) : IEvent
{
    /// <summary>Gets the action identifier supplied by <see cref="Content"/>.</summary>
    public string ActionName => Content.ActionName;
}
