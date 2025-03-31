using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Minecraft.Commands.Brigadier.Suggestion;

namespace Void.Minecraft.Commands.Brigadier.Tree.Nodes;

public class LiteralCommandNode(string literal, CommandExecutor? executor, CommandRequirement? requirement, CommandNode? redirectTarget, RedirectModifier? redirectModifier, bool isForks) : CommandNode(executor, requirement, redirectTarget, redirectModifier, isForks)
{
    private readonly string _literalLowerCase = literal.ToLower();

    public string Literal { get; } = literal;
    public override string Name => Literal;
    public override string UsageText => Literal;
    public override IEnumerable<string> Examples => [Literal];
    protected override string SortedKey => Literal;

    public override IArgumentBuilder<CommandNode> CreateBuilder()
    {
        return LiteralArgumentBuilder.Create(Literal)
            .Requires(Requirement)
            .Forward(RedirectTarget, RedirectModifier, IsForks)
            .Executes(Executor);
    }

    public override bool IsValidInput(string input)
    {
        return Parse(new StringReader(input)) > -1;
    }

    public override async ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken)
    {
        if (_literalLowerCase.StartsWith(builder.RemainingLowerCase))
            return await builder.Suggest(Literal).BuildAsync(cancellationToken);
        else
            return Suggestions.Empty;
    }

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

    private int Parse(StringReader reader)
    {
        var start = reader.Cursor;

        if (reader.CanReadLength(Literal.Length))
        {
            var end = start + Literal.Length;

            if (reader.Source.Substring(start, end) == Literal)
            {
                reader.Cursor = end;

                if (!reader.CanRead || reader.Peek == ' ')
                    return end;
                else
                    reader.Cursor = start;
            }
        }

        return -1;
    }
}
