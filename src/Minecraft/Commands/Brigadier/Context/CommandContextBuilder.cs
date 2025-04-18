using System;
using System.Collections.Generic;
using Void.Minecraft.Commands.Brigadier.Tree;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier.Context;

public class CommandContextBuilder
{
    public ICommandSource Source { get; set; }
    public CommandDispatcher Dispatcher { get; set; }
    public CommandNode RootNode { get; set; }
    public int Start { get; set; }
    public CommandExecutor? Command { get; set; }
    public RedirectModifier? RedirectModifier { get; set; }
    public CommandContextBuilder? Child { get; set; }
    public bool IsFork { get; set; }
    public List<ParsedCommandNode> Nodes { get; } = [];
    public Dictionary<string, IParsedArgument> Arguments { get; } = [];
    public StringRange Range { get; set; }

    public CommandContextBuilder(CommandDispatcher dispatcher, ICommandSource source, CommandNode rootNode, int start)
    {
        Dispatcher = dispatcher;
        Source = source;
        RootNode = rootNode;
        Start = start;
        Range = StringRange.At(Start);
    }

    public CommandContextBuilder WithSource(ICommandSource source)
    {
        Source = source;
        return this;
    }

    public CommandContextBuilder WithArgument<TType>(string name, ParsedArgument<TType> argument)
    {
        Arguments[name] = argument;
        return this;
    }

    public CommandContextBuilder WithExecutor(CommandExecutor? command)
    {
        Command = command;
        return this;
    }

    public CommandContextBuilder WithNode(CommandNode node, StringRange range)
    {

        Nodes.Add(new ParsedCommandNode(node, range));
        Range = StringRange.Encompassing(Range, range);
        RedirectModifier = node.RedirectModifier;
        IsFork = node.IsForks;
        return this;
    }

    public CommandContextBuilder WithChild(CommandContextBuilder child)
    {
        Child = child;
        return this;
    }

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

    public CommandContextBuilder GetLastChild()
    {
        var result = this;

        while (result.Child is not null)
            result = result.Child;

        return result;
    }

    public CommandContext Build(string input)
    {
        if (Source is null)
            throw new InvalidOperationException("Can't build command context without source");

        return new(Source, input, Arguments, Command, RootNode, Nodes, Range, Child?.Build(input), RedirectModifier, IsFork);
    }

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
