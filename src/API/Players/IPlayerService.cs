using System.Net.Sockets;

namespace Void.Proxy.API.Players;

public interface IPlayerService
{
    public IReadOnlyList<IPlayer> All { get; }

    public ValueTask AcceptPlayerAsync(TcpClient client, CancellationToken cancellationToken = default);
    public ValueTask KickPlayerAsync(IPlayer player, string? reason = null, CancellationToken cancellationToken = default);
}