using System.Net.Sockets;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Api.Players;

public interface IPlayer : IEquatable<IPlayer>, ICommandSource, IAsyncDisposable, IDisposable
{
    public TcpClient Client { get; }
    public string RemoteEndPoint { get; }
    public IPlayerContext Context { get; }

    /// <summary>
    /// Computes a stable hash code for the current instance. This is useful when the instance is upgraded or replaced to another implementation.
    /// </summary>
    /// <remarks>The hash code is based on the <see cref="Client"/> property.</remarks>
    /// <returns>An integer representing the hash code of the current instance, derived from the <see cref="Client"/> property.</returns>
    public int GetStableHashCode()
    {
        return Client.GetHashCode();
    }
}
