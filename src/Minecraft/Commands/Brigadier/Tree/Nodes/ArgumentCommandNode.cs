using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Minecraft.Commands.Brigadier.Suggestion;

namespace Void.Minecraft.Commands.Brigadier.Tree.Nodes;

/// <summary>
/// Represents a named argument in a Brigadier command tree.
/// </summary>
/// <remarks>
/// An argument node consumes input by delegating parsing and completion behavior to its configured <see cref="IArgumentType"/>. Parsed values are stored in the active <see cref="CommandContextBuilder"/> under <paramref name="name"/>, and usage text is rendered as <c>&lt;name&gt;</c>.
/// </remarks>
/// <param name="name">The argument name used in command contexts, usage text, and child-node lookup.</param>
/// <param name="type">The argument type that parses input, supplies examples, and provides default suggestions.</param>
/// <param name="executor">The command executor invoked when parsing completes at this node, or <see langword="null"/> when the node is not executable.</param>
/// <param name="requirement">The predicate that controls whether a source can use this node, or <see langword="null"/> to allow all sources.</param>
/// <param name="redirectTarget">The node to redirect execution to, or <see langword="null"/> when this node does not redirect.</param>
/// <param name="redirectModifier">The modifier that maps a command context to redirected sources, or <see langword="null"/> when no redirect modifier is used.</param>
/// <param name="isForks">A value indicating whether redirects from this node fork execution to multiple sources.</param>
/// <param name="customSuggestions">The custom suggestion provider for this argument, or <see langword="null"/> to use <paramref name="type"/> suggestions.</param>
public class ArgumentCommandNode(string name, IArgumentType type, CommandExecutor? executor, CommandRequirement? requirement, CommandNode? redirectTarget, RedirectModifier? redirectModifier, bool isForks, SuggestionProvider? customSuggestions) : CommandNode(executor, requirement, redirectTarget, redirectModifier, isForks)
{
    private const string UsageArgumentOpen = "<";
    private const string UsageArgumentClose = ">";

    public IArgumentType Type { get; } = type;
    public override string Name => name;
    public override string UsageText => $"{UsageArgumentOpen}{Name}{UsageArgumentClose}";
    /// <summary>
    /// Gets representative input strings for this argument node.
    /// </summary>
    /// <remarks>
    /// The sequence is provided by <see cref="IArgumentType.Examples"/> on <see cref="Type"/> and is used by command-tree ambiguity detection to test whether sibling nodes can consume the same sample input.
    /// </remarks>
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
