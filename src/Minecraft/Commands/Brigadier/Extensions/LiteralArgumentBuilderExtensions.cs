using Void.Minecraft.Commands.Brigadier.Builder;

namespace Void.Minecraft.Commands.Brigadier.Extensions;

/// <summary>Provides literal-node factories inside command registration delegates.</summary>
public static class LiteralArgumentBuilderExtensions
{
    /// <summary>Creates a builder for an exact literal command token.</summary>
    /// <param name="_">The placeholder builder context; it is not inspected.</param>
    /// <param name="name">The exact literal.</param>
    /// <returns>The literal builder.</returns>
    public static LiteralArgumentBuilder Literal(this IArgumentContext _, string name)
    {
        return new LiteralArgumentBuilder(name);
    }
}
