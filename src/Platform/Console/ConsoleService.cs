﻿using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Proxy.Api;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Console;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Proxy;
using Void.Terminal;
using SystemConsole = System.Console;

namespace Void.Proxy.Console;

public class ConsoleService(ILogger<ConsoleService> logger, ConsoleConfiguration consoleConfiguration, IRunOptions runOptions, ICommandService commands) : IConsoleService, IEventListener
{
    public static bool IsEnabled => !SystemConsole.IsInputRedirected && !SystemConsole.IsOutputRedirected;
    public IEnumerable<Option> DiscoveredOptions => consoleConfiguration.RootCommand.Options;

    private readonly PromptReader _reader = new();

    [Subscribe(PostOrder.Last)]
    public void OnProxyStarting(ProxyStartingEvent @event)
    {
        if (!consoleConfiguration.HasTerminal)
            return;

        SystemConsole.SetOut(_reader.TextWriter);

        if (!IsEnabled)
            return;

        _reader.ResetStyle();
        _reader.HideCursor();
    }

    [Subscribe]
    public void OnProxyStopping(ProxyStoppingEvent @event)
    {
        if (!consoleConfiguration.HasTerminal)
            return;

        if (!IsEnabled)
            return;

        _reader.ResetStyle();
        _reader.ShowCursor();
    }

    public bool TryGetOptionValue<TValue>(Option<TValue> option, [MaybeNullWhen(false)] out TValue value)
    {
        EnsureOptionDiscovered(option);

        var parseResult = consoleConfiguration.RootCommand.Parse(runOptions.Arguments);
        var optionResult = parseResult.GetResult(option);

        value = parseResult.GetValue(option);
        return value is not null && optionResult is not null && !optionResult.Implicit;
    }

    public TValue? GetOptionValue<TValue>(Option<TValue> option)
    {
        EnsureOptionDiscovered(option);
        return consoleConfiguration.RootCommand.Parse(runOptions.Arguments).GetValue(option);
    }

    public TValue GetRequiredOptionValue<TValue>(Option<TValue> option)
    {
        EnsureOptionDiscovered(option);
        return consoleConfiguration.RootCommand.Parse(runOptions.Arguments).GetRequiredValue(option);
    }

    public void EnsureOptionDiscovered(Option option)
    {
        if (consoleConfiguration.RootCommand.Options.Contains(option))
            return;

        foreach (var existingOption in consoleConfiguration.RootCommand.Options)
        {
            if (!existingOption.Name.Equals(option.Name, StringComparison.OrdinalIgnoreCase) && !existingOption.Aliases.Any(alias => option.Aliases.Contains(alias, StringComparer.OrdinalIgnoreCase)))
                continue;

            throw new InvalidOperationException($"Option with name or alias '{option.Name}' already exists. " +
                $"Discovered: {option.Name} ({string.Join(", ", option.Aliases)}), " +
                $"Existing: {existingOption.Name} ({string.Join(", ", existingOption.Aliases)})");
        }

        var options = consoleConfiguration.RootCommand.Options;

        // Find the position where the new option should be inserted so that the list
        // remains ordered by two keys:
        //   1. Alias count, in descending order (options with more aliases come first)
        //   2. Name, in ascending order, case-insensitive
        //
        // The trick: build a tuple (-AliasCount, NameLowercase). Negating the alias count
        // flips the order so higher counts sort first. We then CompareTo the new option’s
        // tuple and TakeWhile all existing entries that are "less" than it, leaving us
        // with the correct insert index.

        var insertIndex = options.TakeWhile(existing => (-existing.Aliases.Count, existing.Name.ToLowerInvariant()).CompareTo((-option.Aliases.Count, option.Name.ToLowerInvariant())) < 0).Count();
        options.Insert(insertIndex, option);
    }

    public async ValueTask HandleCommandsAsync(CancellationToken cancellationToken = default)
    {
        if (!consoleConfiguration.HasTerminal || !IsEnabled)
        {
            await Task.Delay(5_000, cancellationToken);
            return;
        }

        var command = await _reader.ReadLineAsync(SuggestAsync, cancellationToken);
        logger.LogInformation("Proxy issued command: {command}", command);

        if (string.IsNullOrWhiteSpace(command))
            return;

        try
        {
            await commands.ExecuteAsync(this, command, cancellationToken);
        }
        catch (CommandSyntaxException exception)
        {
            logger.LogError("{Message}", exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError("{Exception}", exception);
        }
    }

    public override string ToString()
    {
        return nameof(Console);
    }

    private async ValueTask<string[]> SuggestAsync(string input, CancellationToken cancellationToken = default)
    {
        return await commands.CompleteAsync(input, this, cancellationToken);
    }
}
