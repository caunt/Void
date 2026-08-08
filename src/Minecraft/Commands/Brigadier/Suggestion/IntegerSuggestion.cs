using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier.Suggestion;

/// <summary>Represents an integer completion formatted with the current culture.</summary>
/// <param name="Range">The replacement range.</param>
/// <param name="Value">The suggested integer.</param>
/// <param name="Tooltip">The optional tooltip.</param>
public record IntegerSuggestion(StringRange Range, int Value, IMessage? Tooltip = null) : Suggestion(Range, Value.ToString(), Tooltip);
