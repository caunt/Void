using System.Net.Sockets;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Player;

public record PlayerConnectingEvent(TcpClient Client, Func<IPlayer, IServiceProvider> GetServices) : IEventWithResult<IPlayer>
{
    public IPlayer? Result { get; set; }
}
