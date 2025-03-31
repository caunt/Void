using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;

namespace Void.Minecraft.Commands.Brigadier.Builder;

public class RequiredArgumentBuilder<TType>(string Name, IArgumentType<TType> Type) : ArgumentBuilder<RequiredArgumentBuilder<TType>, ArgumentCommandNode<TType>>
{
    public ISuggestionProvider? SuggestionProvider { get; private set; }

    public static RequiredArgumentBuilder<TType> Create(string name, IArgumentType<TType> type)
    {
        return new(name, type);
    }

    public override ArgumentCommandNode<TType> Build()
    {
        var result = new ArgumentCommandNode<TType>(Name, Type, Executor, Requirement, RedirectTarget, RedirectModifier, IsForks, SuggestionProvider);

        foreach (var argument in Arguments)
            result.AddChild(argument);

        return result;
    }

    public RequiredArgumentBuilder<TType> Suggests(ISuggestionProvider? provider)
    {
        SuggestionProvider = provider;
        return this;
    }
}
