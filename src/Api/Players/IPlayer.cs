using System.Net.Sockets;
using Void.Common.Commands;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Api.Players;

public interface IPlayer : ICommandSource, IAsyncDisposable
{
    public IPlayerContext Context { get; }
    public TcpClient Client { get; }
    public string RemoteEndPoint { get; }
}
