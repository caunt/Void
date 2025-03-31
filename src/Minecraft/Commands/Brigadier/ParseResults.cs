using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Minecraft.Commands.Brigadier.Tree;

namespace Void.Minecraft.Commands.Brigadier;

public record ParseResults(CommandContextBuilder Context, IImmutableStringReader? Reader = null, Dictionary<CommandNode, CommandSyntaxException>? Exceptions = null);
