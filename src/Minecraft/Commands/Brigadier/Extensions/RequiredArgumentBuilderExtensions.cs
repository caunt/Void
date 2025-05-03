using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Builder;

namespace Void.Minecraft.Commands.Brigadier.Extensions;

public static class RequiredArgumentBuilderExtensions
{
    public static RequiredArgumentBuilder<TType> Argument<TType>(this IArgumentContext _, string name, IArgumentType<TType> type)
    {
        return RequiredArgumentBuilder<TType>.Create(name, type);
    }
}
