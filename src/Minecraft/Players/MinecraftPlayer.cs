using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Minecraft.Players;

public class MinecraftPlayer(TcpClient client, IPlayerContext context, string remoteEndPoint, ProtocolVersion protocolVersion) : IPlayer
{
    public TcpClient Client { get; } = client;
    public IPlayerContext Context { get; } = context;
    public string RemoteEndPoint { get; } = remoteEndPoint;

    public ProtocolVersion ProtocolVersion { get; } = protocolVersion;
    public IdentifiedKey? IdentifiedKey { get; set; } // Only 1.19 - 1.19.2
    public GameProfile? Profile { get; set; }
    public Phase Phase { get; set; }

    public override string ToString()
    {
        return Profile?.Username ?? RemoteEndPoint;
    }

    public bool Equals(IPlayer? other)
    {
        return ((IPlayer)this).GetStableHashCode() == other?.GetStableHashCode();
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
}
