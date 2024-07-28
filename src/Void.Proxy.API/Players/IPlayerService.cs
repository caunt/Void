using System.Net.Sockets;

namespace Void.Proxy.API.Players;

public interface IPlayerService
{
    public IReadOnlyList<IPlayer> All { get; }

    public ValueTask AcceptPlayerAsync(TcpClient client, CancellationToken cancellationToken = default);
}