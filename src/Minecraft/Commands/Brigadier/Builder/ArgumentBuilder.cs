using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Commands.Brigadier.Tree;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;

namespace Void.Minecraft.Commands.Brigadier.Builder;

public interface IArgumentBuilder<out TNode> where TNode : CommandNode
{
    TNode Build();
}

public abstract record ArgumentBuilder
{
    protected readonly RootCommandNode _arguments = new();

    public CommandExecutor? Executor { get; set; }
    public RedirectModifier? RedirectModifier { get; set; }
    public CommandNode? RedirectTarget { get; set; }
    public bool IsForks { get; set; }
    public CommandRequirement? Requirement { get; set; } = (_, _) => ValueTask.FromResult(true);
    public IEnumerable<CommandNode> Arguments => _arguments.Children;

    public abstract CommandNode Build();

    public virtual ArgumentBuilder Executes(CommandExecutor? command)
    {
        Executor = command;
        return this;
    }

    public virtual ArgumentBuilder Executes(CommandExecutorSync? command)
    {
        if (command is not null)
            Executor = (context, _) => ValueTask.FromResult(command(context));

        return this;
    }

    public virtual ArgumentBuilder Requires(CommandRequirement? requirement)
    {
        Requirement = requirement;
        return this;
    }

    public virtual ArgumentBuilder Redirect(CommandNode target)
    {
        return Forward(target, null, false);
    }

    public virtual ArgumentBuilder Redirect(CommandNode target, SingleRedirectModifier modifier)
    {
        return Forward(target, o => [modifier(o)], false);
    }

    public virtual ArgumentBuilder Fork(CommandNode target, RedirectModifier modifier)
    {
        return Forward(target, modifier, true);
    }

    public virtual ArgumentBuilder Forward(CommandNode? target, RedirectModifier? modifier, bool fork)
    {
        if (_arguments.Children.Any())
            throw new InvalidOperationException("Cannot forward a node with children");

        RedirectTarget = target;
        RedirectModifier = modifier;
        IsForks = fork;
        return this;
    }

    public virtual ArgumentBuilder Suggests(SuggestionProvider? provider)
    {
        throw new NotSupportedException($"You have executed this on {GetType()}. Only {nameof(RequiredArgumentBuilder)} supports suggestions.");
    }

    protected void AddChild(CommandNode node)
    {
        if (RedirectTarget is not null)
            throw new InvalidOperationException("Cannot add children to a redirected node");

        _arguments.AddChild(node);
    }
}

public abstract record ArgumentBuilder<TBuilder, TNode> : ArgumentBuilder, IArgumentBuilder<TNode> where TBuilder : ArgumentBuilder<TBuilder, TNode> where TNode : CommandNode
{
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

    public TBuilder Then<TChildNode>(Func<IArgumentContext, IArgumentBuilder<TChildNode>> argument) where TChildNode : CommandNode
    {
        if (RedirectTarget != null)
            throw new InvalidOperationException("Cannot add children to a redirected node");

        _arguments.AddChild(argument(default(ArgumentContext)).Build());
        return GetThis();
    }

    public new TBuilder Executes(CommandExecutor? command)
    {
        return base.Executes(command) as TBuilder ?? GetThis();
    }

    public new TBuilder Executes(CommandExecutorSync? command)
    {
        return base.Executes(command) as TBuilder ?? GetThis();
    }

    public new TBuilder Requires(CommandRequirement? requirement)
    {
        return base.Requires(requirement) as TBuilder ?? GetThis();
    }

    public new TBuilder Redirect(CommandNode target)
    {
        return base.Redirect(target) as TBuilder ?? GetThis();
    }

    public new TBuilder Redirect(CommandNode target, SingleRedirectModifier modifier)
    {
        return base.Redirect(target, modifier) as TBuilder ?? GetThis();
    }

    public new TBuilder Fork(CommandNode target, RedirectModifier modifier)
    {
        return base.Fork(target, modifier) as TBuilder ?? GetThis();
    }

    public new TBuilder Forward(CommandNode? target, RedirectModifier? modifier, bool fork)
    {
        return base.Forward(target, modifier, fork) as TBuilder ?? GetThis();
    }

    public new virtual TBuilder Suggests(SuggestionProvider? provider)
    {
        return base.Suggests(provider) as TBuilder ?? GetThis();
    }

    private TBuilder GetThis()
    {
        if (this is not TBuilder builder)
            throw new InvalidOperationException($"Cannot cast to {nameof(TBuilder)}");

        return builder;
    }

    public abstract override TNode Build();
}
