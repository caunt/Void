using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Builder;

namespace Void.Minecraft.Commands.Brigadier.Extensions;

public static class RequiredArgumentBuilderExtensions
{
    public static RequiredArgumentBuilder Argument(this IArgumentContext _, string name, IArgumentType type)
    {
        return RequiredArgumentBuilder.Create(name, type);
    }
}
