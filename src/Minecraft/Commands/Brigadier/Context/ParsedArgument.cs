using System;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;

namespace Void.Minecraft.Commands.Brigadier.Context;

public interface IParsedArgument
{
    public object GenericResult { get; }
}

public record ParsedArgument(int Start, int End, IArgumentValue Result) : IParsedArgument
{
    public StringRange Range { get; } = new(Start, End);
    public object GenericResult => Result ?? throw new InvalidOperationException($"{nameof(Result)} is null");
}
