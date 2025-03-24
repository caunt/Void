using System.Net.Sockets;
using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Mojang.Profiles;

namespace Void.Proxy.API.Players;

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

    public ValueTask KickAsync(string? reason = null, CancellationToken cancellationToken = default);
}