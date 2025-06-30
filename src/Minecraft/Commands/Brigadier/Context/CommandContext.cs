using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        if (Source?.Equals(source) ?? false)
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

        if (argument.GenericResult is not TType result)
            throw new ArgumentException($"Argument '{name}' is defined as {argument.GenericResult}, not {typeof(TType)}");

        type = result;
        return true;
    }
}
