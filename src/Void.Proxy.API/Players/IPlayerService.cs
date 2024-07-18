using System.Net.Sockets;

namespace Void.Proxy.API.Players;

public interface IPlayerService
{
    public IReadOnlyList<IPlayer> Players { get; }

    public ValueTask AcceptPlayerAsync(TcpClient client);
}