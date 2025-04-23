using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Player;

public record PlayerConnectingEvent(TcpClient Client, AsyncServiceScope Scope, Func<IServiceProvider, IServiceProvider> RegisterEventListeners) : IEventWithResult<IPlayer>
{
    public IPlayer? Result { get; set; }
}
