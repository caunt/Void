using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Components.Text.Events.Actions.Hover;

/// <summary>
/// Represents a hover event action that displays an item stack.
/// </summary>
/// <param name="Id">The item identifier written to the hover event's <c>id</c> field.</param>
/// <param name="Count">The optional stack size. When <see langword="null"/>, the hover event omits an explicit count.</param>
/// <param name="ItemComponents">
/// Optional item component data written to the hover event's <c>components</c> field. When <see langword="null"/>, no component compound is associated with the displayed item.
/// </param>
public record ShowItem(string Id, int? Count = null, NbtCompound? ItemComponents = null) : IHoverEventAction
{
    public string ActionName => "show_item";
}
