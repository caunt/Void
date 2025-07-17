using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Console;
using Void.Terminal;
using SystemConsole = System.Console;

namespace Void.Proxy.Console;

public class ConsoleService(ILogger<ConsoleService> logger, ICommandService commands) : IConsoleService
{
    public static bool IsEnabled => !SystemConsole.IsInputRedirected && !SystemConsole.IsOutputRedirected;

    private PromptReader? _reader;

    public void Setup()
    {
        if (!IsEnabled)
            return;

        _reader ??= new();

        SystemConsole.SetOut(_reader.TextWriter);

        _reader.ResetStyle();
        _reader.HideCursor();
    }

    public async ValueTask HandleCommandsAsync(CancellationToken cancellationToken = default)
    {
        if (!IsEnabled)
        {
            await Task.Delay(5_000, cancellationToken);
            return;
        }

        if (_reader is null)
            throw new InvalidOperationException($"ConsoleService is not set up. Call {nameof(Setup)}() before using this method.");

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
