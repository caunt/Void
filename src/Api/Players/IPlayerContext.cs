using Void.Proxy.Api.Network.IO.Channels;

namespace Void.Proxy.Api.Players;

public interface IPlayerContext : IAsyncDisposable
{
    public IServiceProvider Services { get; }
    public IMinecraftChannel? Channel { get; set; }
}
