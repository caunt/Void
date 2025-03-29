using Void.Proxy.Api.Console;
using Void.Terminal;
using SystemConsole = System.Console;

namespace Void.Proxy.Console;

public class ConsoleService(ILogger<ConsoleService> logger) : IConsoleService
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
        logger.LogInformation("Command: {command}", await _reader.ReadLineAsync(cancellationToken));
    }
}
