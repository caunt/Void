using Void.Minecraft.Commands.Brigadier.Tree;

namespace Void.Minecraft.Commands.Brigadier.Context;

/// <summary>Associates a matched command node with the input range it consumed.</summary>
/// <param name="Node">The matched node.</param>
/// <param name="Range">The consumed range.</param>
public record ParsedCommandNode(CommandNode Node, StringRange Range);
