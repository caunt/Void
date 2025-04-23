using System.Net.Sockets;
using Void.Proxy.Api.Events.Player;

namespace Void.Proxy.Api.Players;

public interface IPlayerService
{
    public IEnumerable<IPlayer> All { get; }

    public void ForEach(Action<IPlayer> action);
    public ValueTask ForEachAsync(Func<IPlayer, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default);
    public ValueTask AcceptPlayerAsync(TcpClient client, CancellationToken cancellationToken = default);
    public ValueTask UpgradeAsync(IPlayer player, IPlayer upgradedPlayer, CancellationToken cancellationToken);
    public ValueTask KickPlayerAsync(IPlayer player, string? text = null, CancellationToken cancellationToken = default);
    public ValueTask KickPlayerAsync(IPlayer player, PlayerKickEvent playerKickEvent, CancellationToken cancellationToken = default);
}
