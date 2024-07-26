using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Players;

public interface IPlayer : IAsyncDisposable
{
    public AsyncServiceScope Scope { get; }
    public TcpClient Client { get; }
    public string RemoteEndPoint { get; }

    public string? Name { get; set; }
    public string? Brand { get; set; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public ClientType ClientType { get; set; }

    ValueTask<IMinecraftChannel> BuildServerChannelAsync(IServer server);
    ValueTask<IMinecraftChannel> GetChannelAsync();
}