using Void.Minecraft.Components.Text.Events;

namespace Void.Minecraft.Components.Text.Properties;

public record Interactivity(string? Insertion = null, ClickEvent? ClickEvent = null, HoverEvent? HoverEvent = null)
{
    public static Interactivity Default { get; } = new();
}
