using Void.Proxy.Api.Commands;

namespace Void.Proxy.Api.Console;

public interface IConsoleService : ICommandSource
{
    /// <summary>
    /// Configures the environment for the application console.
    /// </summary>
    public void Setup();

    /// <summary>
    /// Asynchronously processes single incoming command until cancellation is requested.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will stop processing current command if the token is
    /// canceled.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public ValueTask HandleCommandsAsync(CancellationToken cancellationToken = default);
}
