using Void.Proxy.API.Links;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.Links;

public class LinkService(
    ILogger<LinkService> logger,
    IServerService servers) : ILinkService
{
    private readonly List<ILink> _links = [];

    public async ValueTask ConnectPlayerAnywhereAsync(IPlayer player)
    {
        var server = servers.RegisteredServers.First();

        var playerChannel = await player.GetChannelAsync();
        var serverChannel = await player.BuildServerChannelAsync(server);

        var link = new Link(player, server, playerChannel, serverChannel, FinalizeAsync);
        _links.Add(link);

        logger.LogInformation("Started forwarding {Link} traffic", link);
    }

    private async ValueTask FinalizeAsync(ILink link)
    {
        if (!_links.Remove(link))
            return;

        await link.DisposeAsync();

        logger.LogInformation("Stopped forwarding {Link} traffic", link);
    }
}