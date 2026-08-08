using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Tree;

namespace Void.Minecraft.Commands.Brigadier;

/// <summary>Receives a pair of ambiguous sibling command nodes.</summary>
/// <param name="parent">The common parent.</param>
/// <param name="children">The child whose examples were tested.</param>
/// <param name="sibling">The sibling that accepted them.</param>
/// <param name="inputs">The overlapping example inputs.</param>
public delegate void AmbiguousConsumer(CommandNode parent, CommandNode children, CommandNode sibling, params IEnumerable<string> inputs);
