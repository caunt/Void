using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Tree;

namespace Void.Minecraft.Commands.Brigadier;

public delegate void AmbiguousConsumer(CommandNode parent, CommandNode children, CommandNode sibling, params IEnumerable<string> inputs);
