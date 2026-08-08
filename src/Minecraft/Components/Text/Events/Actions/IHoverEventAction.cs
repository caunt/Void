namespace Void.Minecraft.Components.Text.Events.Actions;

/// <summary>Defines the payload of a text-component hover event.</summary>
public interface IHoverEventAction
{
    /// <summary>Gets the protocol hover-action identifier.</summary>
    public string ActionName { get; }
}
