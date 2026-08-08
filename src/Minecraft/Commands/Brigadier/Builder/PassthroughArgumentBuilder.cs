using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Registry;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;

namespace Void.Minecraft.Commands.Brigadier.Builder;

/// <summary>Builds an argument node that preserves opaque protocol parser properties.</summary>
/// <param name="Identifier">The parser identifier mapping.</param>
/// <param name="Name">The argument name.</param>
/// <param name="Result">The passthrough property value.</param>
public record PassthroughArgumentBuilder(ArgumentSerializerMapping Identifier, string Name, IPassthroughArgumentValue Result) : ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode>
{
    /// <summary>Gets the custom suggestion provider.</summary>
    public SuggestionProvider? SuggestionProvider { get; private set; }

    /// <inheritdoc/>
    public override ArgumentCommandNode Build()
    {
        var argumentType = new PassthroughArgumentType(Identifier, Result);
        var node = new ArgumentCommandNode(Name, argumentType, Executor, Requirement, RedirectTarget, RedirectModifier, IsForks, SuggestionProvider);

        foreach (var argument in Arguments)
            node.AddChild(argument);

        return node;
    }

    /// <inheritdoc/>
    public override PassthroughArgumentBuilder Suggests(SuggestionProvider? provider)
    {
        SuggestionProvider = provider;
        return this;
    }
}
