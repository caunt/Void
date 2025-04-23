using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Void.Minecraft.Network;
using Void.Minecraft.Profiles;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Minecraft.Players;

public class MinecraftPlayer(TcpClient client, IPlayerContext context, ProtocolVersion protocolVersion) : IMinecraftPlayer
{
    public TcpClient Client => client;
    public IPlayerContext Context => context;
    public ProtocolVersion ProtocolVersion => protocolVersion;
    public string RemoteEndPoint => client.Client.RemoteEndPoint?.ToString() ?? "Unknown?";

    // used in 1.19 - 1.19.2
    public IdentifiedKey? IdentifiedKey { get; set; }
    public string? Brand { get; set; }
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
