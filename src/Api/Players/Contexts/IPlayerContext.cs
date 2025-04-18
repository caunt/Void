using Void.Proxy.Api.Network.Channels;

namespace Void.Proxy.Api.Players.Contexts;

public interface IPlayerContext : IAsyncDisposable
{
    public IServiceProvider Services { get; }
    public INetworkChannel? Channel { get; set; }
}
