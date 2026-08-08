using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Tree;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier.Context;

/// <summary>Contains the parsed nodes, arguments, source, and execution metadata for a command stage.</summary>
/// <param name="Source">The command source for this context.</param>
/// <param name="Input">The complete command input.</param>
/// <param name="Arguments">Parsed arguments keyed by node name.</param>
/// <param name="Executor">The executor associated with the final parsed node, if any.</param>
/// <param name="RootNode">The tree root used for parsing.</param>
/// <param name="Nodes">The parsed command-node sequence.</param>
/// <param name="Range">The input range covered by this context.</param>
/// <param name="Child">The redirected child context, if any.</param>
/// <param name="RedirectModifier">The source modifier applied before entering the child.</param>
/// <param name="Forks">Whether this redirect forks execution across sources.</param>
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
    /// <summary>Gets whether at least one command node was parsed.</summary>
    public bool HasNodes => Nodes.Count > 0;

    /// <summary>Returns this context for an equal source, or a shallow copy using another source.</summary>
    /// <param name="source">The source to associate.</param>
    /// <returns>This instance or the source-adjusted copy.</returns>
    public CommandContext CopyFor(ICommandSource source)
    {
        if (Source.Equals(source))
            return this;

        return new CommandContext(source, Input, Arguments, Executor, RootNode, Nodes, Range, Child, RedirectModifier, Forks);
    }

    /// <summary>Traverses redirected children to the final context.</summary>
    /// <returns>The last context in the child chain.</returns>
    public CommandContext GetLastChild()
    {
        var result = this;

        while (result.Child is not null)
            result = result.Child;

        return result;
    }

    /// <summary>Gets and converts a parsed argument value by name.</summary>
    /// <typeparam name="TType">The requested result type.</typeparam>
    /// <param name="name">The argument name.</param>
    /// <returns>The converted argument value.</returns>
    /// <exception cref="ArgumentException">The argument is absent or has an incompatible result type.</exception>
    public TType GetArgument<TType>(string name)
    {
        if (!TryGetArgument<TType>(name, out var type))
            throw new ArgumentException($"No such argument '{name}' exists on this command");

        return type;
    }

    /// <summary>Attempts to get and convert a parsed argument value by name.</summary>
    /// <typeparam name="TType">The requested result type.</typeparam>
    /// <param name="name">The argument name.</param>
    /// <param name="type">The converted value when found.</param>
    /// <returns><see langword="false"/> only when the name is absent; otherwise <see langword="true"/>.</returns>
    /// <exception cref="ArgumentException">The argument exists but its result has an incompatible type.</exception>
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
