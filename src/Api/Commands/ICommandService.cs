using Void.Common.Commands;

namespace Void.Proxy.Api.Commands;

public interface ICommandService
{
    public ICommandDispatcher Dispatcher { get; }

    public ValueTask ExecuteAsync(ICommandSource source, string command, CancellationToken cancellationToken = default);
    public ValueTask<string[]> CompleteAsync(string input, ICommandSource source, CancellationToken cancellationToken = default);
}
