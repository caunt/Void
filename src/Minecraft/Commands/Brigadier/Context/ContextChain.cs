using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Proxy.Api.Commands;

namespace Void.Minecraft.Commands.Brigadier.Context;

/// <summary>Represents redirect modifiers followed by one executable command context.</summary>
/// <param name="Modifiers">The ordered redirect-modifier contexts.</param>
/// <param name="Executable">The final executable context.</param>
public record ContextChain(List<CommandContext> Modifiers, CommandContext Executable)
{
    private ContextChain? _nextStageCache;

    /// <summary>Gets whether this chain begins with modification or execution.</summary>
    public ContextChainStage Stage => Modifiers.Count == 0 ? ContextChainStage.Execute : ContextChainStage.Modify;
    /// <summary>Gets a cached chain with the first modifier removed, or <see langword="null"/> at execution.</summary>
    public ContextChain? NextStage => Modifiers.Count is 0 ? null : _nextStageCache ??= new ContextChain(Modifiers.Slice(1, Modifiers.Count), Executable);
    /// <summary>Gets the first modifier, or the executable context when no modifiers remain.</summary>
    public CommandContext TopContext => Modifiers.Count == 0 ? Executable : Modifiers[0];

    // public ContextChain()
    // {
    //     if (Executable.Command == null)
    //         throw new InvalidOperationException("Last command in chain must be executable");
    // }

    /// <summary>Flattens a redirected child chain into modifiers and one executable context.</summary>
    /// <param name="rootContext">The root parsed context.</param>
    /// <returns>The flattened chain, or <see langword="null"/> when the final context has no executor.</returns>
    public static ContextChain? TryFlatten(CommandContext rootContext)
    {
        var modifiers = new List<CommandContext>();
        var current = rootContext;

        while (true)
        {
            var child = current.Child;
            if (child == null)
            {
                // Last entry must be executable command
                if (current.Executor == null)
                {
                    return null;
                }

                return new ContextChain(modifiers, current);
            }

            modifiers.Add(current);
            current = child;
        }
    }

    /// <summary>Runs one redirect modifier or propagates the source for a simple redirect.</summary>
    /// <param name="modifier">The redirect context.</param>
    /// <param name="source">The current source.</param>
    /// <param name="resultConsumer">The callback notified of syntax failures.</param>
    /// <param name="forkedMode">Whether syntax failures should be converted to an empty source sequence.</param>
    /// <returns>The sources for the next stage.</returns>
    public static IEnumerable<ICommandSource> RunModifier(CommandContext modifier, ICommandSource source, ResultConsumer resultConsumer, bool forkedMode)
    {
        var sourceModifier = modifier.RedirectModifier;

        // Note: source currently in context is irrelevant at this point, since we might have updated it in one of the earlier stages
        if (sourceModifier == null)
        {
            // Simple redirect, just propagate source to next node
            return [source];
        }

        var contextToUse = modifier.CopyFor(source);
        try
        {
            return sourceModifier(contextToUse);
        }
        catch (CommandSyntaxException)
        {
            resultConsumer(contextToUse, false, 0);

            if (forkedMode)
                return [];

            throw;
        }
    }

    /// <summary>Executes one final context and reports its outcome.</summary>
    /// <param name="context">The executable context.</param>
    /// <param name="source">The execution source.</param>
    /// <param name="resultConsumer">The callback notified of success or syntax failure.</param>
    /// <param name="forkedMode">Whether to return one per successful branch and suppress syntax failures.</param>
    /// <param name="cancellationToken">The cancellation token passed to the executor.</param>
    /// <returns>The executor result, one for a successful fork, or zero for a suppressed failure.</returns>
    public static async ValueTask<int> RunExecutableAsync(CommandContext context, ICommandSource source, ResultConsumer resultConsumer, bool forkedMode, CancellationToken cancellationToken)
    {
        var contextToUse = context.CopyFor(source);
        try
        {
            if (context.Executor is null)
                throw new InvalidOperationException("Last command in chain must be executable");

            var result = await context.Executor(contextToUse, cancellationToken);
            resultConsumer(contextToUse, true, result);
            return forkedMode ? 1 : result;
        }
        catch (CommandSyntaxException)
        {
            resultConsumer(contextToUse, false, 0);

            if (forkedMode)
                return 0;

            throw;
        }
    }

    /// <summary>Runs all redirect stages and executes the final context for every resulting source.</summary>
    /// <param name="source">The initial source.</param>
    /// <param name="resultConsumer">The callback notified for each execution attempt.</param>
    /// <param name="cancellationToken">The cancellation token passed to executors.</param>
    /// <returns>The accumulated result, using successful branch counts once the chain forks.</returns>
    public async ValueTask<int> ExecuteAllAsync(ICommandSource source, ResultConsumer resultConsumer, CancellationToken cancellationToken)
    {
        if (Modifiers.Count == 0)
        {
            // Fast path - just a single stage
            return await RunExecutableAsync(Executable, source, resultConsumer, false, cancellationToken);
        }

        var forkedMode = false;
        var currentSources = new List<ICommandSource>() { source };

        foreach (var modifier in Modifiers)
        {
            forkedMode |= modifier.Forks;

            var nextSources = new List<ICommandSource>();
            foreach (var sourceToRun in currentSources)
            {
                nextSources.AddRange(RunModifier(modifier, sourceToRun, resultConsumer, forkedMode));
            }
            if (nextSources.Count == 0)
            {
                return 0;
            }
            currentSources = nextSources;
        }

        var result = 0;

        foreach (var executionSource in currentSources)
            result += await RunExecutableAsync(Executable, executionSource, resultConsumer, forkedMode, cancellationToken);

        return result;
    }

    /// <summary>Identifies the operation performed by the current chain stage.</summary>
    public enum ContextChainStage
    {
        /// <summary>Apply a redirect source modifier.</summary>
        Modify,
        /// <summary>Invoke the final command executor.</summary>
        Execute,
    }
}
