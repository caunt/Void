using Void.Minecraft.Commands.Brigadier.Context;

namespace Void.Minecraft.Commands.Brigadier.Suggestion;

public record IntegerSuggestion(StringRange Range, int Value, IMessage? Tooltip = null) : Suggestion(Range, Value.ToString(), Tooltip);
