using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;

namespace Void.Minecraft.Commands.Brigadier.Builder;

/// <summary>Builds a named, parsed argument command node.</summary>
/// <param name="Name">The argument name.</param>
/// <param name="Type">The parser and completion behavior.</param>
public record RequiredArgumentBuilder(string Name, IArgumentType Type) : ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode>
{
    /// <summary>Gets the custom suggestion provider, or <see langword="null"/> to use the argument type.</summary>
    public SuggestionProvider? SuggestionProvider { get; private set; }

    /// <summary>Creates a required-argument builder.</summary>
    /// <param name="name">The argument name.</param>
    /// <param name="type">The argument type.</param>
    /// <returns>The builder.</returns>
    public static RequiredArgumentBuilder Create(string name, IArgumentType type)
    {
        return new(name, type);
    }

    /// <inheritdoc/>
    public override ArgumentCommandNode Build()
    {
        var result = new ArgumentCommandNode(Name, Type, Executor, Requirement, RedirectTarget, RedirectModifier, IsForks, SuggestionProvider);

        foreach (var argument in Arguments)
            result.AddChild(argument);

        return result;
    }

    /// <inheritdoc/>
    public override RequiredArgumentBuilder Suggests(SuggestionProvider? provider)
    {
        SuggestionProvider = provider;
        return this;
    }

    /// <summary>Wraps and sets a synchronous suggestion provider.</summary>
    /// <param name="provider">The provider; <see langword="null"/> leaves the current provider unchanged.</param>
    /// <returns>This builder.</returns>
    public RequiredArgumentBuilder Suggests(SuggestionProviderSync? provider)
    {
        if (provider is not null)
            SuggestionProvider = (context, builder, _) => ValueTask.FromResult(provider(context, builder));

        return this;
    }
}
