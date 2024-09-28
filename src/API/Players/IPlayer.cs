using System.Net.Sockets;
using Void.Proxy.API.Network.Protocol;

namespace Void.Proxy.API.Players;

public interface IPlayer : IAsyncDisposable
{
    public IPlayerContext Context { get; }

    public TcpClient Client { get; }
    public string RemoteEndPoint { get; }

    public string? Name { get; set; }
    public string? Brand { get; set; }
    public ProtocolVersion ProtocolVersion { get; set; }
}