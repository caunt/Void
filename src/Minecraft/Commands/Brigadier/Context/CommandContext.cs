using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Tree;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier.Context;

public record CommandContext(
    ICommandSource Source,
    string Input,
    Dictionary<string, IParsedArgument> Arguments,
    CommandExecutor? Executor,
    CommandNode RootNode,
    List<ParsedCommandNode> Nodes,
    StringRange Range,
    CommandContext? Child,
    RedirectModifier? RedirectModifier,
    bool Forks
    )
{
    public bool HasNodes => Nodes.Count > 0;

    public CommandContext CopyFor(ICommandSource source)
    {
        if (Source.Equals(source))
            return this;

        return new CommandContext(source, Input, Arguments, Executor, RootNode, Nodes, Range, Child, RedirectModifier, Forks);
    }

    public CommandContext GetLastChild()
    {
        var result = this;

        while (result.Child is not null)
            result = result.Child;

        return result;
    }

    public TType GetArgument<TType>(string name)
    {
        if (!TryGetArgument<TType>(name, out var type))
            throw new ArgumentException($"No such argument '{name}' exists on this command");

        return type;
    }

    public bool TryGetArgument<TType>(string name, [MaybeNullWhen(false)] out TType type)
    {
        type = default;

        if (!Arguments.TryGetValue(name, out var argument))
            return false;

        switch (argument.Result)
        {
            case BoolArgumentValue boolValue when typeof(TType) == typeof(bool):
                type = (TType)(object)boolValue.Value;
                return true;
            case IntegerArgumentValue intValue when typeof(TType) == typeof(int):
                type = (TType)(object)intValue.Value;
                return true;
            case FloatArgumentValue floatValue when typeof(TType) == typeof(float):
                type = (TType)(object)floatValue.Value;
                return true;
            case LongArgumentValue longValue when typeof(TType) == typeof(long):
                type = (TType)(object)longValue.Value;
                return true;
            case DoubleArgumentValue doubleValue when typeof(TType) == typeof(double):
                type = (TType)(object)doubleValue.Value;
                return true;
            case StringArgumentValue stringValue when typeof(TType) == typeof(string):
                type = (TType)(object)stringValue.Value;
                return true;
            case TType result:
                type = result;
                return true;
            default:
                throw new ArgumentException($"Argument '{name}' is defined as {argument.Result.GetType()}, not {typeof(TType)}");
        }
    }
}
