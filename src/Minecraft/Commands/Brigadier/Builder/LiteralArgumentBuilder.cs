using Void.Minecraft.Commands.Brigadier.Tree.Nodes;

namespace Void.Minecraft.Commands.Brigadier.Builder;

public record LiteralArgumentBuilder(string Literal) : ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode>
{
    public static LiteralArgumentBuilder Create(string value)
    {
        return new(value);
    }

    public override LiteralCommandNode Build()
    {
        var result = new LiteralCommandNode(Literal, Executor, Requirement, RedirectTarget, RedirectModifier, IsForks);

        foreach (var argument in Arguments)
            result.AddChild(argument);

        return result;
    }
}
