using Void.Proxy.Api.Servers;

namespace Void.Proxy.Servers;

public class ServerService : IServerService
{
    private readonly List<IServer> _servers = [];
    public IReadOnlyList<IServer> RegisteredServers => _servers;

    public void RegisterServer(IServer server)
    {
        _servers.Add(server);
    }
}
