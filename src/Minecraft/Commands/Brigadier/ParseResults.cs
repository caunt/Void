using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Minecraft.Commands.Brigadier.Tree;

namespace Void.Minecraft.Commands.Brigadier;

/// <summary>Contains the best command parse branch, its final reader state, and rejected sibling exceptions.</summary>
/// <param name="Context">The accumulated parse context.</param>
/// <param name="Reader">The final reader state.</param>
/// <param name="Exceptions">Recoverable parse failures keyed by candidate node.</param>
public record ParseResults(CommandContextBuilder Context, IImmutableStringReader Reader, Dictionary<CommandNode, CommandSyntaxException> Exceptions)
{
    /// <summary>Creates an empty-input result around a context builder.</summary>
    /// <param name="context">The parse context.</param>
    public ParseResults(CommandContextBuilder context) : this(context, new StringReader(string.Empty), [])
    {
        // Empty
    }
}
