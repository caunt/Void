using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Suggestion;
using Void.Minecraft.Commands.Brigadier.Tree;
using Void.Minecraft.Commands.Brigadier.Tree.Nodes;

namespace Void.Minecraft.Commands.Brigadier.Builder;

/// <summary>Builds a command-tree node.</summary>
/// <typeparam name="TNode">The produced node type.</typeparam>
public interface IArgumentBuilder<out TNode> where TNode : CommandNode
{
    /// <summary>Creates the configured command node.</summary>
    /// <returns>The built node.</returns>
    TNode Build();
}

/// <summary>Provides executor, requirement, child, and redirect configuration shared by command-node builders.</summary>
public abstract record ArgumentBuilder
{
    /// <summary>Stores child nodes until a concrete builder creates its node.</summary>
    protected readonly RootCommandNode _arguments = new();

    /// <summary>Gets or sets the asynchronous executor.</summary>
    public CommandExecutor? Executor { get; set; }
    /// <summary>Gets or sets the redirect source modifier.</summary>
    public RedirectModifier? RedirectModifier { get; set; }
    /// <summary>Gets or sets the redirect target.</summary>
    public CommandNode? RedirectTarget { get; set; }
    /// <summary>Gets or sets whether redirected execution forks.</summary>
    public bool IsForks { get; set; }
    /// <summary>Gets or sets the asynchronous access requirement; the default permits every source.</summary>
    public CommandRequirement? Requirement { get; set; } = (_, _) => ValueTask.FromResult(true);
    /// <summary>Gets the configured child nodes.</summary>
    public IEnumerable<CommandNode> Arguments => _arguments.Children;

    /// <summary>Creates the configured command node.</summary>
    /// <returns>The built node.</returns>
    public abstract CommandNode Build();

    /// <summary>Sets the asynchronous command executor.</summary>
    /// <param name="command">The executor, or <see langword="null"/>.</param>
    /// <returns>This builder.</returns>
    public virtual ArgumentBuilder Executes(CommandExecutor? command)
    {
        Executor = command;
        return this;
    }

    /// <summary>Wraps and sets a synchronous command executor.</summary>
    /// <param name="command">The executor; <see langword="null"/> leaves the current executor unchanged.</param>
    /// <returns>This builder.</returns>
    public virtual ArgumentBuilder Executes(CommandExecutorSync? command)
    {
        if (command is not null)
            Executor = (context, _) => ValueTask.FromResult(command(context));

        return this;
    }

    /// <summary>Sets the command-source access requirement.</summary>
    /// <param name="requirement">The requirement, or <see langword="null"/> to allow every source when the node is built.</param>
    /// <returns>This builder.</returns>
    public virtual ArgumentBuilder Requires(CommandRequirement? requirement)
    {
        Requirement = requirement;
        return this;
    }

    /// <summary>Configures a simple redirect that retains the current source.</summary>
    /// <param name="target">The redirect target.</param>
    /// <returns>This builder.</returns>
    public virtual ArgumentBuilder Redirect(CommandNode target)
    {
        return Forward(target, null, false);
    }

    /// <summary>Configures a redirect that maps each context to one source.</summary>
    /// <param name="target">The redirect target.</param>
    /// <param name="modifier">The single-source modifier.</param>
    /// <returns>This builder.</returns>
    public virtual ArgumentBuilder Redirect(CommandNode target, SingleRedirectModifier modifier)
    {
        return Forward(target, o => [modifier(o)], false);
    }

    /// <summary>Configures a redirect that may produce multiple execution sources.</summary>
    /// <param name="target">The redirect target.</param>
    /// <param name="modifier">The source-sequence modifier.</param>
    /// <returns>This builder.</returns>
    public virtual ArgumentBuilder Fork(CommandNode target, RedirectModifier modifier)
    {
        return Forward(target, modifier, true);
    }

    /// <summary>Sets redirect metadata after verifying that the builder has no children.</summary>
    /// <param name="target">The redirect target, or <see langword="null"/>.</param>
    /// <param name="modifier">The source modifier, or <see langword="null"/> for a simple redirect.</param>
    /// <param name="fork">Whether execution forks.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="InvalidOperationException">The builder already contains children.</exception>
    public virtual ArgumentBuilder Forward(CommandNode? target, RedirectModifier? modifier, bool fork)
    {
        if (_arguments.Children.Any())
            throw new InvalidOperationException("Cannot forward a node with children");

        RedirectTarget = target;
        RedirectModifier = modifier;
        IsForks = fork;
        return this;
    }

    /// <summary>Sets a custom suggestion provider when supported by the concrete builder.</summary>
    /// <param name="provider">The provider.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="NotSupportedException">The concrete builder does not support suggestions.</exception>
    public virtual ArgumentBuilder Suggests(SuggestionProvider? provider)
    {
        throw new NotSupportedException($"You have executed this on {GetType()}. Only {nameof(RequiredArgumentBuilder)} supports suggestions.");
    }

    /// <summary>Adds a child node to this builder.</summary>
    /// <param name="node">The child node.</param>
    /// <exception cref="InvalidOperationException">A redirect target is already configured.</exception>
    protected void AddChild(CommandNode node)
    {
        if (RedirectTarget is not null)
            throw new InvalidOperationException("Cannot add children to a redirected node");

        _arguments.AddChild(node);
    }
}

/// <summary>Provides fluent, strongly typed operations for a command-node builder.</summary>
/// <typeparam name="TBuilder">The concrete builder type.</typeparam>
/// <typeparam name="TNode">The produced node type.</typeparam>
public abstract record ArgumentBuilder<TBuilder, TNode> : ArgumentBuilder, IArgumentBuilder<TNode> where TBuilder : ArgumentBuilder<TBuilder, TNode> where TNode : CommandNode
{
    /// <summary>Builds and adds a child node.</summary>
    /// <typeparam name="TChildNode">The child node type.</typeparam>
    /// <param name="argument">The child builder.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="InvalidOperationException">A redirect target is already configured.</exception>
    public TBuilder Then<TChildNode>(IArgumentBuilder<TChildNode> argument) where TChildNode : CommandNode
    {
        if (RedirectTarget is not null)
            throw new InvalidOperationException("Cannot add children to a redirected node");

        _arguments.AddChild(argument.Build());
        return GetThis();
    }

    /// <summary>Adds an existing child node.</summary>
    /// <typeparam name="TChildNode">A command-node type used for fluent overload selection.</typeparam>
    /// <param name="node">The child node.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="InvalidOperationException">A redirect target is already configured.</exception>
    public TBuilder Then<TChildNode>(CommandNode node) where TChildNode : CommandNode
    {
        if (RedirectTarget is not null)
            throw new InvalidOperationException("Cannot add children to a redirected node");

        _arguments.AddChild(node);
        return GetThis();
    }

    /// <summary>Invokes a child-builder factory with the default argument context, builds it, and adds the result.</summary>
    /// <typeparam name="TChildNode">The child node type.</typeparam>
    /// <param name="argument">The child-builder factory.</param>
    /// <returns>This builder.</returns>
    /// <exception cref="InvalidOperationException">A redirect target is already configured.</exception>
    public TBuilder Then<TChildNode>(Func<IArgumentContext, IArgumentBuilder<TChildNode>> argument) where TChildNode : CommandNode
    {
        if (RedirectTarget != null)
            throw new InvalidOperationException("Cannot add children to a redirected node");

        _arguments.AddChild(argument(default(ArgumentContext)).Build());
        return GetThis();
    }

    /// <summary>Sets an asynchronous executor and returns the concrete builder type.</summary>
    /// <param name="command">The executor, or <see langword="null"/>.</param>
    /// <returns>This builder.</returns>
    public new TBuilder Executes(CommandExecutor? command)
    {
        return base.Executes(command) as TBuilder ?? GetThis();
    }

    /// <summary>Sets a synchronous executor and returns the concrete builder type.</summary>
    /// <param name="command">The executor, or <see langword="null"/> to leave it unchanged.</param>
    /// <returns>This builder.</returns>
    public new TBuilder Executes(CommandExecutorSync? command)
    {
        return base.Executes(command) as TBuilder ?? GetThis();
    }

    /// <summary>
    /// Sets the predicate that determines whether a command source can use the node produced by this builder.
    /// </summary>
    /// <param name="requirement">The asynchronous requirement delegate to store on the built command node, or <see langword="null"/> to allow every source.</param>
    /// <returns>The current builder instance, typed as <typeparamref name="TBuilder"/>.</returns>
    public new TBuilder Requires(CommandRequirement? requirement)
    {
        return base.Requires(requirement) as TBuilder ?? GetThis();
    }

    /// <summary>Configures a simple redirect and returns the concrete builder type.</summary>
    /// <param name="target">The redirect target.</param>
    /// <returns>This builder.</returns>
    public new TBuilder Redirect(CommandNode target)
    {
        return base.Redirect(target) as TBuilder ?? GetThis();
    }

    /// <summary>
    /// Redirects execution to <paramref name="target"/> and uses <paramref name="modifier"/> to map the redirected context to a single command source.
    /// </summary>
    /// <param name="target">The command node to redirect to.</param>
    /// <param name="modifier">The callback used to produce the redirected command source for each execution context.</param>
    /// <returns>The current builder instance, typed as <typeparamref name="TBuilder"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the builder already contains child nodes and cannot be redirected.</exception>
    public new TBuilder Redirect(CommandNode target, SingleRedirectModifier modifier)
    {
        return base.Redirect(target, modifier) as TBuilder ?? GetThis();
    }

    /// <summary>Configures a forking redirect and returns the concrete builder type.</summary>
    /// <param name="target">The redirect target.</param>
    /// <param name="modifier">The source modifier.</param>
    /// <returns>This builder.</returns>
    public new TBuilder Fork(CommandNode target, RedirectModifier modifier)
    {
        return base.Fork(target, modifier) as TBuilder ?? GetThis();
    }

    /// <summary>Sets redirect metadata and returns the concrete builder type.</summary>
    /// <param name="target">The redirect target.</param>
    /// <param name="modifier">The source modifier.</param>
    /// <param name="fork">Whether execution forks.</param>
    /// <returns>This builder.</returns>
    public new TBuilder Forward(CommandNode? target, RedirectModifier? modifier, bool fork)
    {
        return base.Forward(target, modifier, fork) as TBuilder ?? GetThis();
    }

    /// <summary>Sets a suggestion provider when supported and returns the concrete builder type.</summary>
    /// <param name="provider">The provider.</param>
    /// <returns>This builder.</returns>
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

    /// <inheritdoc/>
    public abstract override TNode Build();
}
