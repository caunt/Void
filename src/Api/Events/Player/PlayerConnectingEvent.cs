using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;
using Void.Common.Events;

namespace Void.Proxy.Api.Events.Player;

public record PlayerConnectingEvent(TcpClient Client, IServiceCollection Services) : IEvent;
