using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Minecraft.Commands.Brigadier.Suggestion;

namespace Void.Minecraft.Commands.Brigadier.Tree.Nodes;

public class ArgumentCommandNode(string name, IArgumentType type, CommandExecutor? executor, CommandRequirement? requirement, CommandNode? redirectTarget, RedirectModifier? redirectModifier, bool isForks, SuggestionProvider? customSuggestions) : CommandNode(executor, requirement, redirectTarget, redirectModifier, isForks)
{
    private const string UsageArgumentOpen = "<";
    private const string UsageArgumentClose = ">";

    public IArgumentType Type { get; } = type;
    public override string Name => name;
    public override string UsageText => $"{UsageArgumentOpen}{Name}{UsageArgumentClose}";
    public override IEnumerable<string> Examples => Type.Examples;
    protected override string SortedKey => Name;
    public SuggestionProvider? CustomSuggestions { get; set; } = customSuggestions;

    public override IArgumentBuilder<CommandNode> CreateBuilder()
    {
        return RequiredArgumentBuilder.Create(Name, Type)
            .Requires(Requirement)
            .Forward(RedirectTarget, RedirectModifier, IsForks)
            .Suggests(CustomSuggestions)
            .Executes(Executor);
    }

    public override bool IsValidInput(string input)
    {
        try
        {
            var reader = new StringReader(input);
            Type.Parse(reader);
            return !reader.CanRead || reader.Peek is ' ';
        }
        catch (CommandSyntaxException)
        {
            return false;
        }
    }

    public override async ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken)
    {
        if (CustomSuggestions is null)
            return await Type.ListSuggestionsAsync(context, builder, cancellationToken);
        else
            return await CustomSuggestions(context, builder, cancellationToken);
    }

    public override void Parse(StringReader reader, CommandContextBuilder contextBuilder)
    {
        var start = reader.Cursor;
        var result = Type.Parse(reader);
        var parsed = new ParsedArgument(start, reader.Cursor, result);

        contextBuilder.WithArgument(Name, parsed);
        contextBuilder.WithNode(this, parsed.Range);
    }

    public override string ToString()
    {
        return $"ArgumentCommandNode{{name='{Name}', type={Type}}}";
    }
}
