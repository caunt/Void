using Void.Minecraft.Commands.Brigadier.Tree.Nodes;

namespace Void.Minecraft.Commands.Brigadier.Builder;

/// <summary>Builds a command node that matches a fixed literal.</summary>
/// <param name="Literal">The exact literal to match.</param>
public record LiteralArgumentBuilder(string Literal) : ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode>
{
    /// <summary>Creates a literal-node builder.</summary>
    /// <param name="value">The exact literal.</param>
    /// <returns>The builder.</returns>
    public static LiteralArgumentBuilder Create(string value)
    {
        return new(value);
    }

    /// <inheritdoc/>
    public override LiteralCommandNode Build()
    {
        var result = new LiteralCommandNode(Literal, Executor, Requirement, RedirectTarget, RedirectModifier, IsForks);

        foreach (var argument in Arguments)
            result.AddChild(argument);

        return result;
    }
}
