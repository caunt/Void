using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Minecraft.Players;

public class MinecraftPlayer(TcpClient client, IPlayerContext context, string remoteEndPoint, ProtocolVersion protocolVersion) : IMinecraftPlayer
{
    public TcpClient Client { get; } = client;
    public IPlayerContext Context { get; } = context;
    public string RemoteEndPoint { get; } = remoteEndPoint;

    public ProtocolVersion ProtocolVersion { get; } = protocolVersion;
    public IdentifiedKey? IdentifiedKey { get; set; } // Only 1.19 - 1.19.2
    public GameProfile? Profile { get; set; }
    public Phase Phase { get; set; }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override string ToString()
    {
        return Profile?.Username ?? RemoteEndPoint;
    }
}
