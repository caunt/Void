using Void.Minecraft.Commands.Brigadier.Builder;

namespace Void.Minecraft.Commands.Brigadier.Extensions;

public static class LiteralArgumentBuilderExtensions
{
    public static LiteralArgumentBuilder Literal(this IArgumentContext _, string name)
    {
        return new LiteralArgumentBuilder(name);
    }
}
