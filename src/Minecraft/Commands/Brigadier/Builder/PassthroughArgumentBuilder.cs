using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Registry;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;

namespace Void.Minecraft.Commands.Brigadier.Builder;

public record PassthroughArgumentBuilder(ArgumentSerializerMapping Identifier, string Name, IPassthroughArgumentValue Result) : ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode>
{
    public SuggestionProvider? SuggestionProvider { get; private set; }

    public override ArgumentCommandNode Build()
    {
        var argumentType = new PassthroughArgumentType(Identifier, Result);
        var node = new ArgumentCommandNode(Name, argumentType, Executor, Requirement, RedirectTarget, RedirectModifier, IsForks, SuggestionProvider);

        foreach (var argument in Arguments)
            node.AddChild(argument);

        return node;
    }

    public override PassthroughArgumentBuilder Suggests(SuggestionProvider? provider)
    {
        SuggestionProvider = provider;
        return this;
    }
}
