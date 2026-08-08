using Void.Minecraft.Profiles;

namespace Void.Minecraft.Components.Text.Events.Actions.Hover;

/// <summary>Represents a hover action that displays entity information.</summary>
/// <param name="Id">The entity UUID.</param>
/// <param name="Type">The optional entity-type resource location.</param>
/// <param name="Name">The optional display-name component.</param>
public record ShowEntity(Uuid Id, string? Type = null, Component? Name = null) : IHoverEventAction
{
    /// <summary>Gets the <c>show_entity</c> hover-action identifier.</summary>
    public string ActionName => "show_entity";
}
