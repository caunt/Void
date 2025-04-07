using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Common.Commands;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Suggestion;

namespace Void.Minecraft.Commands.Brigadier.Tree.Nodes;

public class RootCommandNode() : CommandNode(requirement: EmptyRequirement, redirectModifier: static context => [context.Source])
{
    public override string Name => string.Empty;
    public override string UsageText => string.Empty;
    protected override string SortedKey => string.Empty;

    public override IEnumerable<string> Examples => [];

    public override IArgumentBuilder<CommandNode> CreateBuilder()
    {
        throw new InvalidOperationException("Cannot convert root into a builder");
    }

    public override bool IsValidInput(string input)
    {
        return false;
    }

    public override ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Suggestions.Empty);
    }

    public override void Parse(StringReader reader, CommandContextBuilder context)
    {
        // Empty
    }

    private static ValueTask<bool> EmptyRequirement(ICommandSource source, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(true);
    }
}
