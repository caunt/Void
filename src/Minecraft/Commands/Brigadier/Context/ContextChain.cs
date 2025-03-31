using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Void.Minecraft.Commands.Brigadier.Exceptions;

namespace Void.Minecraft.Commands.Brigadier.Context;

public record ContextChain(List<CommandContext> Modifiers, CommandContext Executable)
{
    private ContextChain? _nextStageCache;

    public ContextChainStage Stage => Modifiers.Count == 0 ? ContextChainStage.Execute : ContextChainStage.Modify;
    public ContextChain? NextStage => Modifiers.Count is 0 ? null : _nextStageCache ??= new ContextChain(Modifiers.Slice(1, Modifiers.Count), Executable);
    public CommandContext TopContext => Modifiers.Count == 0 ? Executable : Modifiers[0];

    // public ContextChain()
    // {
    //     if (Executable.Command == null)
    //         throw new InvalidOperationException("Last command in chain must be executable");
    // }

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

    public static IEnumerable<ICommandSource> RunModifier(CommandContext modifier, ICommandSource source, ResultConsumer resultConsumer, bool forkedMode)
    {
        var sourceModifier = modifier.RedirectModifier;

        // Note: source currently in context is irrelevant at this point, since we might have updated it in one of earlier stages
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

    public enum ContextChainStage
    {
        Modify,
        Execute,
    }
}
