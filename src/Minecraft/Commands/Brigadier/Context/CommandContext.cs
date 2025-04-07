using System;
using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Tree;

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
        if (!Arguments.TryGetValue(name, out var argument))
            throw new ArgumentException($"No such argument '{name}' exists on this command");

        if (argument.GenericResult is not TType result)
            throw new ArgumentException($"Argument {name}' is defined as {argument.GenericResult}, not {typeof(TType)}");

        return result;
    }
}
