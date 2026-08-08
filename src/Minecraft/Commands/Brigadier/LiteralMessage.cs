namespace Void.Minecraft.Commands.Brigadier;

/// <summary>Represents an immutable literal command message.</summary>
/// <param name="Value">The message text.</param>
public record LiteralMessage(string Value) : IMessage;
