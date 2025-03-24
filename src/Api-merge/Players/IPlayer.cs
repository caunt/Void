using System.Net.Sockets;
using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Mojang.Profiles;

namespace Void.Proxy.Api.Players;

public interface IPlayer : IAsyncDisposable
{
    public IPlayerContext Context { get; }

    public TcpClient Client { get; }
    public string RemoteEndPoint { get; }

    public string? Brand { get; set; }
    public ProtocolVersion ProtocolVersion { get; set; }
    public GameProfile? Profile { get; set; }
    public IdentifiedKey? IdentifiedKey { get; set; }
    public Phase Phase { get; set; }
}