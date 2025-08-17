using Void.Minecraft.Commands.Brigadier.Exceptions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Console;
using Void.Terminal;
using SystemConsole = System.Console;

namespace Void.Proxy.Console;

public class ConsoleService(ILogger<ConsoleService> logger, ConsoleRedirectConfiguration consoleRedirectConfiguration, ICommandService commands) : IConsoleService
{
    public static bool IsEnabled => !SystemConsole.IsInputRedirected && !SystemConsole.IsOutputRedirected;

    private readonly PromptReader _reader = new();

    public void Setup()
    {
        if (consoleRedirectConfiguration.TextWriter is not null)
            return;

        SystemConsole.SetOut(_reader.TextWriter);

        if (!IsEnabled)
            return;

        _reader.ResetStyle();
        _reader.HideCursor();
    }

    public async ValueTask HandleCommandsAsync(CancellationToken cancellationToken = default)
    {
        if (consoleRedirectConfiguration.TextWriter is not null || !IsEnabled)
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
