using Void.Common.Commands;

namespace Void.Proxy.Api.Console;

public interface IConsoleService : ICommandSource
{
    public void Setup();
    public ValueTask HandleCommandsAsync(CancellationToken cancellationToken = default);
}
