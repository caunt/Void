namespace Void.Proxy.API.Servers;

public interface IServerService
{
    public IReadOnlyList<IServer> RegisteredServers { get; }

    public void RegisterServer(IServer server);
}