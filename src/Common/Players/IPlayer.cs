using System;
using System.Net.Sockets;
using Void.Proxy.Api.Players;

namespace Void.Common.Players;

public interface IPlayer : IAsyncDisposable
{
    public IPlayerContext Context { get; }
    public TcpClient Client { get; }
    public string RemoteEndPoint { get; }
}
