using Void.Minecraft.Components.Text.Events;

namespace Void.Minecraft.Components.Text.Properties;

/// <summary>Describes optional interactive behavior attached to a text component.</summary>
/// <param name="Insertion">The text inserted into chat on shift-click, or <see langword="null"/> when absent.</param>
/// <param name="ClickEvent">The action invoked when the component is clicked, or <see langword="null"/>.</param>
/// <param name="HoverEvent">The action displayed when the component is hovered, or <see langword="null"/>.</param>
public record Interactivity(string? Insertion = null, ClickEvent? ClickEvent = null, HoverEvent? HoverEvent = null)
{
    /// <summary>Gets interactivity with no insertion, click event, or hover event.</summary>
    public static Interactivity Default { get; } = new();
}
