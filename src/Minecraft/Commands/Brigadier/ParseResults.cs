using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Minecraft.Commands.Brigadier.Tree;

namespace Void.Minecraft.Commands.Brigadier;

public record ParseResults(CommandContextBuilder Context, IImmutableStringReader Reader, Dictionary<CommandNode, CommandSyntaxException> Exceptions)
{
    public ParseResults(CommandContextBuilder context) : this(context, new StringReader(string.Empty), [])
    {
        // Empty
    }
}
