using Void.Common.Commands;
using Void.Minecraft.Commands.Brigadier;
using Void.Proxy.Api.Commands;

namespace Void.Proxy.Commands;

public class CommandService : ICommandService
{
    private readonly CommandDispatcher _dispatcher = new();

    public ICommandDispatcher Dispatcher => _dispatcher;

    public async ValueTask ExecuteAsync(ICommandSource source, string command, CancellationToken cancellationToken = default)
    {
        if (command.StartsWith('/'))
            command = command[1..];

        _ = await _dispatcher.ExecuteAsync(command, source, cancellationToken);
    }

    public async ValueTask<string[]> CompleteAsync(string input, ICommandSource source, CancellationToken cancellationToken = default)
    {
        var suggestions = await _dispatcher.SuggestAsync(input, source, cancellationToken);
        return [.. suggestions.All.Select(suggestion => suggestion.Text)];
    }
}
