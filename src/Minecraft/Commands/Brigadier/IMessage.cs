namespace Void.Minecraft.Commands.Brigadier;

/// <summary>Defines displayable command feedback text.</summary>
public interface IMessage
{
    /// <summary>Gets the message text.</summary>
    public string Value { get; }
}
