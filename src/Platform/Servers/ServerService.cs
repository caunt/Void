using Void.Proxy.Api.Links;
using Void.Proxy.Api.Servers;
using Void.Proxy.Api.Settings;

namespace Void.Proxy.Servers;

public class ServerService(ISettings settings, ILinkService links) : IServerService
{
    public IEnumerable<IServer> All => settings.Servers.Concat(links.All.Select(link => link.Server));
}
