namespace Void.Minecraft.Components.Text.Events.Actions;

/// <summary>Defines the payload of a text-component click event.</summary>
public interface IClickEventAction
{
    /// <summary>Gets the protocol click-action identifier.</summary>
    public string ActionName { get; }
}
