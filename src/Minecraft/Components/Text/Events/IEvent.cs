namespace Void.Minecraft.Components.Text.Events;

/// <summary>Defines a serialized text-component interaction event.</summary>
public interface IEvent
{
    /// <summary>Gets the protocol action identifier.</summary>
    public string ActionName { get; }
}
