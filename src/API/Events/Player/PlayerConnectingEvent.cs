using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;

namespace Void.Proxy.API.Events.Player;

public class PlayerConnectingEvent : IEvent
{
    public required TcpClient Client { get; init; }
    public required IServiceCollection Services { get; init; }
}