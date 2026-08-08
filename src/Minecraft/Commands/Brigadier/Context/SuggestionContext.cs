using Void.Minecraft.Commands.Brigadier.Tree;

namespace Void.Minecraft.Commands.Brigadier.Context;

/// <summary>Identifies the parent node and replacement start used to build completions.</summary>
/// <param name="Parent">The node whose children may provide suggestions.</param>
/// <param name="Start">The cursor at which the suggested token begins.</param>
public record SuggestionContext(CommandNode Parent, int Start);
