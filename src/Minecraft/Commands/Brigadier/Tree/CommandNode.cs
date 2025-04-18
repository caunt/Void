using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier.Tree;

public abstract class CommandNode(CommandExecutor? executor = null, CommandRequirement? requirement = null, CommandNode? redirectTarget = null, RedirectModifier? redirectModifier = null, bool isForks = false) : ICommandNode
{
    private readonly Dictionary<string, CommandNode> _children = [];
    private readonly Dictionary<string, ArgumentCommandNode> _arguments = [];
    private readonly Dictionary<string, LiteralCommandNode> _literals = [];

    public bool IsForks { get; set; } = isForks;
    public CommandRequirement? Requirement { get; set; } = requirement;
    public CommandExecutor? Executor { get; set; } = executor;
    public CommandNode? RedirectTarget { get; set; } = redirectTarget;
    public RedirectModifier? RedirectModifier { get; set; } = redirectModifier;
    public IEnumerable<CommandNode> Children => _children.Values;
    public abstract string Name { get; }
    public abstract string UsageText { get; }
    public abstract IEnumerable<string> Examples { get; }
    protected abstract string SortedKey { get; }

    public void AddChild(CommandNode node)
    {
        if (node is RootCommandNode)
            throw new InvalidOperationException("Cannot add root node as child.");

        if (_children.TryGetValue(node.Name, out var child))
        {
            if (node.Executor is not null)
                child.Executor = node.Executor;

            foreach (var grandChild in node.Children)
                child.AddChild(grandChild);
        }
        else
        {
            _children[node.Name] = node;

            if (node is LiteralCommandNode literal)
                _literals[node.Name] = literal;
            else if (node is ArgumentCommandNode argument)
                _arguments[node.Name] = argument;
        }
    }

    public async ValueTask<bool> CanUseAsync(ICommandSource source, CancellationToken cancellationToken)
    {
        if (Requirement is null)
            return true;

        return await Requirement(source, cancellationToken);
    }

    public void FindAmbiguities(AmbiguousConsumer consumer)
    {
        var matches = new HashSet<string>();

        foreach (var child in Children)
        {
            foreach (var sibling in Children)
            {
                if (child == sibling)
                    continue;

                foreach (var input in child.Examples)
                {
                    if (sibling.IsValidInput(input))
                        matches.Add(input);
                }

                if (matches.Count > 0)
                {
                    consumer(this, child, sibling, matches);
                    matches.Clear();
                }
            }

            child.FindAmbiguities(consumer);
        }
    }

    public CommandNode GetChild(string name)
    {
        return _children[name];
    }

    public IEnumerable<CommandNode> GetRelevantNodes(StringReader reader)
    {
        if (_literals.Count > 0)
        {
            var cursor = reader.Cursor;

            while (reader.CanRead && reader.Peek != ' ')
                reader.Skip();

            var text = reader.Source[cursor..reader.Cursor];
            reader.Cursor = cursor;

            if (_literals.TryGetValue(text, out var literal))
                return [literal];
            else
                return _arguments.Values;
        }
        else
        {
            return _arguments.Values;
        }
    }

    public abstract IArgumentBuilder<CommandNode> CreateBuilder();
    public abstract bool IsValidInput(string input);
    public abstract ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken);
    public abstract void Parse(StringReader reader, CommandContextBuilder context);
}
