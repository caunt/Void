using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.Api.Network.Channels;

namespace Void.Proxy.Api.Players.Contexts;

public interface IPlayerContext : IDisposable, IAsyncDisposable
{
    public IPlayer Player { get; }
    public IServiceProvider Services { get; }
    public INetworkChannel? Channel { get; set; }
    public AsyncServiceScope Scope { get; }
}
