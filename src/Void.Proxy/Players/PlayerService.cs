using System.Net.Sockets;
using Void.Proxy.API.Links;
using Void.Proxy.API.Players;

namespace Void.Proxy.Players;

public class PlayerService(
    ILogger<PlayerService> logger,
    IServiceScopeFactory serviceScopeFactory,
    ILinkService links) : IPlayerService
{
    private readonly List<Player> _players = [];
    public IReadOnlyList<IPlayer> Players => _players;

    public async ValueTask AcceptPlayerAsync(TcpClient client)
    {
        var remoteEndPoint = client.Client?.RemoteEndPoint?.ToString() ?? "Unknown?";

        try
        {
            var scope = serviceScopeFactory.CreateAsyncScope();
            var player = new Player(scope, client, remoteEndPoint);

            _players.Add(player);
            await links.ConnectPlayerAnywhereAsync(player);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Client {RemoteEndPoint} cannot be proxied", remoteEndPoint);
        }
    }
}