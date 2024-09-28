using System.Net.Sockets;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Players;

namespace Void.Proxy.Players;

public class Player(TcpClient client, IPlayerContext context) : IPlayer
{
    public IPlayerContext Context => context;
    public TcpClient Client => client;
    public string RemoteEndPoint { get; } = client.Client.RemoteEndPoint?.ToString() ?? "Unknown?";

    public string? Name { get; set; }
    public string? Brand { get; set; }

    public ProtocolVersion ProtocolVersion { get; set; } = ProtocolVersion.Oldest; // we do not know Player protocol version yet, use the oldest possible

    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public override string ToString()
    {
        return Name ?? RemoteEndPoint;
    }
}