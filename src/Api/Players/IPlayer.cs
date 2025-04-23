using System.Net.Sockets;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Api.Players;

public interface IPlayer : ICommandSource, IAsyncDisposable, IDisposable
{
    public TcpClient Client { get; }
    public string RemoteEndPoint { get; }
    public IPlayerContext Context { get; }
}
