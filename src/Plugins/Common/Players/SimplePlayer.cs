using System.Net.Sockets;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Plugins.Common.Players;

public class SimplePlayer : IPlayer
{
    public TcpClient Client { get; }
    public IPlayerContext Context { get; }
    public string RemoteEndPoint { get; }

    public SimplePlayer(TcpClient client, Func<IPlayer, IPlayerContext> contextBuilder)
    {
        Client = client;
        RemoteEndPoint = client.Client.RemoteEndPoint?.ToString() ?? "Unknown?";

        Context = contextBuilder(this);
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        Context.Dispose();
        GC.SuppressFinalize(this);
    }

    public override string ToString()
    {
        return RemoteEndPoint;
    }
}
