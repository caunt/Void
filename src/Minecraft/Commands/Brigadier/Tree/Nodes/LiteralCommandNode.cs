using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Minecraft.Commands.Brigadier.Suggestion;

namespace Void.Minecraft.Commands.Brigadier.Tree.Nodes;

/// <summary>Represents a command node that consumes one exact literal token.</summary>
/// <param name="literal">The case-sensitive literal.</param>
/// <param name="executor">The command executor.</param>
/// <param name="requirement">The source access requirement.</param>
/// <param name="redirectTarget">The redirect target.</param>
/// <param name="redirectModifier">The redirected-source modifier.</param>
/// <param name="isForks">Whether redirection forks.</param>
public class LiteralCommandNode(string literal, CommandExecutor? executor, CommandRequirement? requirement, CommandNode? redirectTarget, RedirectModifier? redirectModifier, bool isForks) : CommandNode(executor, requirement, redirectTarget, redirectModifier, isForks)
{
    /// <summary>Gets the exact literal consumed by this node.</summary>
    public string Literal { get; } = literal;
    /// <inheritdoc/>
    public override string Name => Literal;
    /// <inheritdoc/>
    public override string UsageText => Literal;
    /// <inheritdoc/>
    public override IEnumerable<string> Examples => [Literal];
    /// <inheritdoc/>
    protected override string SortedKey => Literal;

    /// <inheritdoc/>
    public override IArgumentBuilder<CommandNode> CreateBuilder()
    {
        return LiteralArgumentBuilder.Create(Literal)
            .Requires(Requirement)
            .Forward(RedirectTarget, RedirectModifier, IsForks)
            .Executes(Executor);
    }

    /// <inheritdoc/>
    public override bool IsValidInput(string input)
    {
        return Parse(new StringReader(input)) > -1;
    }

    /// <inheritdoc/>
    public override async ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken)
    {
        if (Literal.StartsWith(builder.Remaining, StringComparison.OrdinalIgnoreCase))
            return await builder.Suggest(Literal).BuildAsync(cancellationToken);
        else
            return Suggestions.Empty;
    }

    /// <inheritdoc/>
    public override void Parse(StringReader reader, CommandContextBuilder contextBuilder)
    {
        var start = reader.Cursor;
        var end = Parse(reader);

        if (end > -1)
        {
            contextBuilder.WithNode(this, StringRange.Between(start, end));
            return;
        }

        throw CommandSyntaxException.BuiltInExceptions.LiteralIncorrect.CreateWithContext(reader, Literal);
    }

    /// <summary>Returns a diagnostic representation containing the literal.</summary>
    /// <returns>The diagnostic representation.</returns>
    public override string ToString()
    {
        return $"LiteralCommandNode{{literal='{Literal}'}}";
    }

    private int Parse(StringReader reader)
    {
        var start = reader.Cursor;

        if (reader.CanReadLength(Literal.Length))
        {
            var end = start + Literal.Length;

            if (reader.Source.AsSpan(start, Literal.Length).Equals(Literal, StringComparison.Ordinal))
            {
                reader.Cursor = end;

                if (!reader.CanRead || reader.Peek is ' ')
                    return end;
                else
                    reader.Cursor = start;
            }
        }

        return -1;
    }
}
