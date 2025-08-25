using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Proxy.Api;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Console;
using Void.Terminal;
using SystemConsole = System.Console;

namespace Void.Proxy.Console;

public class ConsoleService(ILogger<ConsoleService> logger, ConsoleConfiguration consoleConfiguration, IRunOptions runOptions, ICommandService commands) : IConsoleService
{
    public static bool IsEnabled => !SystemConsole.IsInputRedirected && !SystemConsole.IsOutputRedirected;
    public IEnumerable<Option> DiscoveredOptions => consoleConfiguration.RootCommand.Options;

    private readonly PromptReader _reader = new();

    public void Setup()
    {
        if (!consoleConfiguration.HasTerminal)
            return;

        SystemConsole.SetOut(_reader.TextWriter);

        if (!IsEnabled)
            return;

        _reader.ResetStyle();
        _reader.HideCursor();
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

        consoleConfiguration.RootCommand.Options.Add(option);
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
