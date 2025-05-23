namespace Void.Proxy.Api.Servers;

public interface IServerService
{
    public IEnumerable<IServer> All { get; }
    public ValueTask<IServer> GetOrAddServerAsync(string host, int port, CancellationToken cancellationToken = default);
}
