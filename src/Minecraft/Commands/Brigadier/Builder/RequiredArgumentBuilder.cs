using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;

namespace Void.Minecraft.Commands.Brigadier.Builder;

public record RequiredArgumentBuilder(string Name, IArgumentType Type) : ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode>
{
    public SuggestionProvider? SuggestionProvider { get; private set; }

    public static RequiredArgumentBuilder Create(string name, IArgumentType type)
    {
        return new(name, type);
    }

    public override ArgumentCommandNode Build()
    {
        var result = new ArgumentCommandNode(Name, Type, Executor, Requirement, RedirectTarget, RedirectModifier, IsForks, SuggestionProvider);

        foreach (var argument in Arguments)
            result.AddChild(argument);

        return result;
    }

    public override RequiredArgumentBuilder Suggests(SuggestionProvider? provider)
    {
        SuggestionProvider = provider;
        return this;
    }

    public RequiredArgumentBuilder Suggests(SuggestionProviderSync? provider)
    {
        if (provider is not null)
            SuggestionProvider = (context, builder, _) => ValueTask.FromResult(provider(context, builder));

        return this;
    }
}
