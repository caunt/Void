using System.Net.Sockets;
using Void.Minecraft.Components.Text;

namespace Void.Proxy.Api.Players;

public interface IPlayerService
{
    public IReadOnlyList<IPlayer> All { get; }

    public ValueTask AcceptPlayerAsync(TcpClient client, CancellationToken cancellationToken = default);
    public ValueTask KickPlayerAsync(IPlayer player, Component? reason = null, CancellationToken cancellationToken = default);
}