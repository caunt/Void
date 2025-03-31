using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Console;
using Void.Terminal;
using SystemConsole = System.Console;

namespace Void.Proxy.Console;

public class ConsoleService(ILogger<ConsoleService> logger, ICommandService commands) : IConsoleService
{
    private readonly PromptReader _reader = new();

    public void Setup()
    {
        SystemConsole.SetOut(_reader.TextWriter);

        _reader.ResetStyle();
        _reader.HideCursor();
    }

    public async ValueTask HandleCommandsAsync(CancellationToken cancellationToken = default)
    {
        var command = await _reader.ReadLineAsync(cancellationToken);
        logger.LogInformation("Proxy issued command: {command}", command);

        if (!string.IsNullOrWhiteSpace(command))
            await commands.ExecuteAsync(this, command, cancellationToken);
    }
}
