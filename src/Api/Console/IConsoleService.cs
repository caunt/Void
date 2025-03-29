namespace Void.Proxy.Api.Console;

public interface IConsoleService
{
    public void Setup();
    public ValueTask HandleCommandsAsync(CancellationToken cancellationToken = default);
}
