using Void.Proxy.Api.Links;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;

namespace Void.Proxy.Servers;

public class ServerService(ISettings settings, ILinkService links) : IServerService
{
    private readonly List<IServer> _addedServers = new();
    private readonly ISettings _settings = settings;
    private readonly ILinkService _links = links;

    public IEnumerable<IServer> All => _settings.Servers.Concat(_links.All.Select(link => link.Server)).Concat(_addedServers).Distinct();

    public ValueTask<IServer> GetOrAddServerAsync(string host, int port, CancellationToken cancellationToken = default)
    {
        // Check settings servers
        var existingServer = _settings.Servers.FirstOrDefault(s => s.Host == host && s.Port == port);
        if (existingServer != null)
        {
            return ValueTask.FromResult(existingServer);
        }

        // Check servers from active links
        existingServer = _links.All.Select(link => link.Server).FirstOrDefault(s => s.Host == host && s.Port == port);
        if (existingServer != null)
        {
            return ValueTask.FromResult(existingServer);
        }

        // Check already added servers
        existingServer = _addedServers.FirstOrDefault(s => s.Host == host && s.Port == port);
        if (existingServer != null)
        {
            return ValueTask.FromResult(existingServer);
        }

        // If not found, create and add a new one
        var newServerName = $"{host}:{port}";
        var newServer = new Server(newServerName, host, port);
        _addedServers.Add(newServer);

        return ValueTask.FromResult<IServer>(newServer);
    }
}
