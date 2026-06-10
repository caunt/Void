using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Builder;

namespace Void.Minecraft.Commands.Brigadier.Extensions;

public static class RequiredArgumentBuilderExtensions
{
    /// <summary>
    /// Creates a <see cref="RequiredArgumentBuilder"/> for a named command argument.
    /// </summary>
    /// <remarks>
    /// <para>The <paramref name="_"/> receiver is not inspected; it only makes the factory available inside command registration delegates.</para>
    /// <para>This method delegates to <see cref="RequiredArgumentBuilder.Create(string, IArgumentType)"/> and does not perform additional runtime validation.</para>
    /// </remarks>
    /// <param name="_">The command argument context used to scope the builder factory.</param>
    /// <param name="name">The argument name that will be assigned to the resulting command node.</param>
    /// <param name="type">The parser and suggestion behavior for the argument value.</param>
    /// <returns>A required argument builder configured with <paramref name="name"/> and <paramref name="type"/>.</returns>
    public static RequiredArgumentBuilder Argument(this IArgumentContext _, string name, IArgumentType type)
    {
        return RequiredArgumentBuilder.Create(name, type);
    }
}
