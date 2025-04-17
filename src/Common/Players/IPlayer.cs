using System;
using System.Net.Sockets;
using Void.Common.Commands;

namespace Void.Common.Players;

public interface IPlayer : ICommandSource, IAsyncDisposable
{
    public IPlayerContext Context { get; }
    public TcpClient Client { get; }
    public string RemoteEndPoint { get; }
}
