using System.Net.Sockets;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Players;
using Void.Proxy.Players.Contexts;

namespace Void.Proxy.Players;

public class Player(TcpClient client) : IPlayer
{
    // used in 1.19 - 1.19.2
    public IdentifiedKey? IdentifiedKey { get; set; }
    public IPlayerContext Context { get; internal set; } = new EmptyPlayerContext();
    public TcpClient Client => client;
    public string RemoteEndPoint { get; } = client.Client.RemoteEndPoint?.ToString() ?? "Unknown?";

    public string? Brand { get; set; }
    public GameProfile? Profile { get; set; }
    public Phase Phase { get; set; }

    // we do not know Player protocol version yet, use the oldest possible
    public ProtocolVersion ProtocolVersion { get; set; } = ProtocolVersion.Oldest;

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public override string ToString()
    {
        return Profile?.Username ?? RemoteEndPoint;
    }
}