namespace Void.Minecraft.Components.Text.Events.Actions.Hover;

/// <summary>Represents a hover action that displays a text component.</summary>
/// <param name="Value">The component displayed by the client.</param>
public record ShowText(Component Value) : IHoverEventAction
{
    /// <summary>Gets the <c>show_text</c> hover-action identifier.</summary>
    public string ActionName => "show_text";
}
