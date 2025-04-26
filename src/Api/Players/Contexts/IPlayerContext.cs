using Void.Proxy.Api.Network.Channels;

namespace Void.Proxy.Api.Players.Contexts;

public interface IPlayerContext : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the current player instance.
    /// </summary>
    public IPlayer Player { get; }

    /// <summary>
    /// Gets the service provider that provides access to player scoped services.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// Gets or sets the network channel used for communication.
    /// </summary>
    public INetworkChannel? Channel { get; set; }

    /// <summary>
    /// Gets the current state of the player context.
    /// </summary>
    public bool IsDisposed { get; }
}
