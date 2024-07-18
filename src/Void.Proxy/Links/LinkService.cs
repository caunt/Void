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
        link.StartForwarding();

        _links.Add(link);
    }

    private async ValueTask FinalizeAsync(ILink link)
    {
        if (!_links.Remove(link))
            return;

        logger.LogInformation("Link {Player}=>{Server} disposed", link.Player, link.Server);
        await link.DisposeAsync();
    }
}