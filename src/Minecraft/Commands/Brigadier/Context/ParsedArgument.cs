using System;

namespace Void.Minecraft.Commands.Brigadier.Context;

public interface IParsedArgument
{
    public object GenericResult { get; }
}

public record ParsedArgument<TType>(int Start, int End, TType Result) : IParsedArgument
{
    public StringRange Range { get; } = new(Start, End);
    public object GenericResult => Result ?? throw new InvalidOperationException($"{nameof(Result)} is null");
}
