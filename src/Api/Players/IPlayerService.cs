using System.Net.Sockets;
using Void.Common.Players;
using Void.Proxy.Api.Events.Player;

namespace Void.Proxy.Api.Players;

public interface IPlayerService
{
    public IReadOnlyList<IPlayer> All { get; }

    public ValueTask AcceptPlayerAsync(TcpClient client, CancellationToken cancellationToken = default);
    public ValueTask KickPlayerAsync(IPlayer player, string? text = null, CancellationToken cancellationToken = default);
    public ValueTask KickPlayerAsync(IPlayer player, PlayerKickEvent playerKickEvent, CancellationToken cancellationToken = default);
}
