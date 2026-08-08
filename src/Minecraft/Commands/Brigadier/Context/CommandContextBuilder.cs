using System;
using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Tree;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier.Context;

/// <summary>Accumulates parse results for one stage of a potentially redirected command.</summary>
public class CommandContextBuilder
{
    /// <summary>Gets or sets the command source.</summary>
    public ICommandSource Source { get; set; }
    /// <summary>Gets or sets the dispatcher that owns the parsed tree.</summary>
    public CommandDispatcher Dispatcher { get; set; }
    /// <summary>Gets or sets the parsing root node.</summary>
    public CommandNode RootNode { get; set; }
    /// <summary>Gets or sets the initial parse cursor.</summary>
    public int Start { get; set; }
    /// <summary>Gets or sets the executor selected by parsing.</summary>
    public CommandExecutor? Command { get; set; }
    /// <summary>Gets or sets the redirect source modifier.</summary>
    public RedirectModifier? RedirectModifier { get; set; }
    /// <summary>Gets or sets the redirected child context builder.</summary>
    public CommandContextBuilder? Child { get; set; }
    /// <summary>Gets or sets whether the redirect forks across sources.</summary>
    public bool IsFork { get; set; }
    /// <summary>Gets the mutable parsed-node sequence.</summary>
    public List<ParsedCommandNode> Nodes { get; } = [];
    /// <summary>Gets the mutable parsed-argument map.</summary>
    public Dictionary<string, IParsedArgument> Arguments { get; } = [];
    /// <summary>Gets or sets the aggregate input range covered by parsed nodes.</summary>
    public StringRange Range { get; set; }

    /// <summary>Creates an empty context builder at a parse position.</summary>
    /// <param name="dispatcher">The dispatcher owning the tree.</param>
    /// <param name="source">The command source.</param>
    /// <param name="rootNode">The parsing root.</param>
    /// <param name="start">The initial cursor.</param>
    public CommandContextBuilder(CommandDispatcher dispatcher, ICommandSource source, CommandNode rootNode, int start)
    {
        Dispatcher = dispatcher;
        Source = source;
        RootNode = rootNode;
        Start = start;
        Range = StringRange.At(Start);
    }

    /// <summary>Replaces the command source.</summary>
    /// <param name="source">The new source.</param>
    /// <returns>This builder.</returns>
    public CommandContextBuilder WithSource(ICommandSource source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    /// Records a parsed argument by name for the command context being built.
    /// </summary>
    /// <param name="name">The argument name from the command tree.</param>
    /// <param name="argument">The parsed argument range and value to associate with <paramref name="name"/>.</param>
    /// <returns>The current <see cref="CommandContextBuilder"/> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <remarks>If an argument with the same <paramref name="name"/> was already recorded, it is replaced.</remarks>
    public CommandContextBuilder WithArgument(string name, ParsedArgument argument)
    {
        Arguments[name] = argument;
        return this;
    }

    /// <summary>Associates an executor with this stage.</summary>
    /// <param name="command">The executor, or <see langword="null"/> to clear it.</param>
    /// <returns>This builder.</returns>
    public CommandContextBuilder WithExecutor(CommandExecutor? command)
    {
        Command = command;
        return this;
    }

    /// <summary>Appends a parsed node and adopts its redirect metadata.</summary>
    /// <param name="node">The parsed node.</param>
    /// <param name="range">The consumed range.</param>
    /// <returns>This builder.</returns>
    public CommandContextBuilder WithNode(CommandNode node, StringRange range)
    {
        Nodes.Add(new ParsedCommandNode(node, range));
        Range = StringRange.Encompassing(Range, range);
        RedirectModifier = node.RedirectModifier;
        IsFork = node.IsForks;
        return this;
    }

    /// <summary>Sets the redirected child stage.</summary>
    /// <param name="child">The child builder.</param>
    /// <returns>This builder.</returns>
    public CommandContextBuilder WithChild(CommandContextBuilder child)
    {
        Child = child;
        return this;
    }

    /// <summary>Creates a shallow copy with new node and argument collections.</summary>
    /// <returns>The copied builder. Its child, source, nodes, and argument values remain shared objects.</returns>
    public CommandContextBuilder Copy()
    {
        var copy = new CommandContextBuilder(Dispatcher, Source, RootNode, Start)
        {
            Source = Source,
            Command = Command,
            RedirectModifier = RedirectModifier,
            Child = Child,
            IsFork = IsFork,
            Range = Range
        };

        copy.Nodes.AddRange(Nodes);

        foreach (var (key, value) in Arguments)
            copy.Arguments.Add(key, value);

        return copy;
    }

    /// <summary>Traverses redirected children to the final builder.</summary>
    /// <returns>The last builder in the child chain.</returns>
    public CommandContextBuilder GetLastChild()
    {
        var result = this;

        while (result.Child is not null)
            result = result.Child;

        return result;
    }

    /// <summary>Creates an immutable command context and recursively builds its child.</summary>
    /// <param name="input">The complete command input.</param>
    /// <returns>The built context.</returns>
    /// <exception cref="InvalidOperationException"><see cref="Source"/> is <see langword="null"/>.</exception>
    public CommandContext Build(string input)
    {
        if (Source is null)
            throw new InvalidOperationException($"Can't build command context without {nameof(Source)}");

        return new(Source, input, Arguments, Command, RootNode, Nodes, Range, Child?.Build(input), RedirectModifier, IsFork);
    }

    /// <summary>Locates the command-tree position at which suggestions should be requested.</summary>
    /// <param name="cursor">The completion cursor.</param>
    /// <returns>The parent node and token start.</returns>
    /// <exception cref="InvalidOperationException">No node precedes the cursor within this context or its children.</exception>
    public SuggestionContext BuildSuggestions(int cursor)
    {
        if (Range.Start <= cursor)
        {
            if (Range.End < cursor)
            {
                if (Child is not null)
                    return Child.BuildSuggestions(cursor);

                if (Nodes.Count > 0)
                {
                    var last = Nodes[^1];
                    return new SuggestionContext(last.Node, last.Range.End + 1);
                }

                return new SuggestionContext(RootNode, Range.Start);
            }
            else
            {
                var prev = RootNode;

                foreach (var node in Nodes)
                {
                    var nodeRange = node.Range;

                    if (nodeRange.Start <= cursor && cursor <= nodeRange.End)
                        return new SuggestionContext(prev, nodeRange.Start);

                    prev = node.Node;
                }

                if (prev is null)
                    throw new InvalidOperationException("Can't find node before cursor");

                return new SuggestionContext(prev, Range.Start);
            }
        }

        throw new InvalidOperationException("Can't find node before cursor");
    }
}
