using Void.Minecraft.Nbt.Tags;

namespace Void.Minecraft.Components.Text.Events.Actions.Hover;

public record ShowItem(string Id, int? Count = 0, NbtCompound? ItemComponents = null) : IHoverEventAction
{
    public string ActionName => "show_item";
}
