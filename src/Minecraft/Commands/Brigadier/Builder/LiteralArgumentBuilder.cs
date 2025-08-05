using Void.Minecraft.Commands.Brigadier.Tree.Nodes;

namespace Void.Minecraft.Commands.Brigadier.Builder;

public class LiteralArgumentBuilder(string literal) : ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode>
{
    public static LiteralArgumentBuilder Create(string value)
    {
        return new(value);
    }

    public override LiteralCommandNode Build()
    {
        var result = new LiteralCommandNode(literal, Executor, Requirement, RedirectTarget, RedirectModifier, IsForks);

        foreach (var argument in Arguments)
            result.AddChild(argument);

        return result;
    }
}
