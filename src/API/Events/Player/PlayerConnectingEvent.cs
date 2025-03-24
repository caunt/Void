using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;

namespace Void.Proxy.Api.Events.Player;

public record PlayerConnectingEvent(TcpClient Client, IServiceCollection Services) : IEvent;