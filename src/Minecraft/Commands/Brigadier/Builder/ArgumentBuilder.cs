using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Tree;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;

namespace Void.Minecraft.Commands.Brigadier.Builder;

public interface IArgumentBuilder<out TNode> where TNode : CommandNode
{
    TNode Build();
}

public abstract class ArgumentBuilder<TBuilder, TNode> : IArgumentBuilder<TNode> where TBuilder : ArgumentBuilder<TBuilder, TNode> where TNode : CommandNode
{
    private readonly RootCommandNode _arguments = new();

    public CommandExecutor? Executor { get; set; }
    public RedirectModifier? RedirectModifier { get; set; }
    public CommandNode? RedirectTarget { get; set; }
    public bool IsForks { get; set; }
    public CommandRequirement? Requirement { get; set; } = _ => ValueTask.FromResult(true);
    public IEnumerable<CommandNode> Arguments => _arguments.Children;

    public TBuilder Then<TChildNode>(IArgumentBuilder<TChildNode> argument) where TChildNode : CommandNode
    {
        if (RedirectTarget is not null)
            throw new InvalidOperationException("Cannot add children to a redirected node");

        _arguments.AddChild(argument.Build());
        return GetThis();
    }

    public TBuilder Then<TChildNode>(CommandNode node) where TChildNode : CommandNode
    {
        if (RedirectTarget is not null)
            throw new InvalidOperationException("Cannot add children to a redirected node");

        _arguments.AddChild(node);
        return GetThis();
    }

    public TBuilder Executes(CommandExecutor? command)
    {
        Executor = command;
        return GetThis();
    }

    public TBuilder Requires(CommandRequirement? requirement)
    {
        Requirement = requirement;
        return GetThis();
    }

    public TBuilder Redirect(CommandNode target)
    {
        return Forward(target, null, false);
    }

    public TBuilder Redirect(CommandNode target, SingleRedirectModifier modifier)
    {
        return Forward(target, o => [modifier(o)], false);
    }

    public TBuilder Fork(CommandNode target, RedirectModifier modifier)
    {
        return Forward(target, modifier, true);
    }

    public TBuilder Forward(CommandNode? target, RedirectModifier? modifier, bool fork)
    {
        if (_arguments.Children.Any())
            throw new InvalidOperationException("Cannot forward a node with children");

        RedirectTarget = target;
        RedirectModifier = modifier;
        IsForks = fork;
        return GetThis();
    }

    private TBuilder GetThis()
    {
        if (this is not TBuilder builder)
            throw new InvalidOperationException($"Cannot cast to {nameof(TBuilder)}");

        return builder;
    }

    public abstract TNode Build();
}
