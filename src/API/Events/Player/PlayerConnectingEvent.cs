using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;

namespace Void.Proxy.API.Events.Player;

public class PlayerConnectingEvent : IEvent
{
    public required TcpClient Client { get; init; }
    public required IServiceCollection Services { get; init; }
}