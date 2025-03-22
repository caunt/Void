using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Network.IO.Channels.Services;

public interface IChannelBuilderService
{
    public bool IsFallbackBuilder { get; }

    public ValueTask SearchChannelBuilderAsync(IPlayer player, CancellationToken cancellationToken = default);

    public ValueTask<IMinecraftChannel> BuildPlayerChannelAsync(IPlayer player, CancellationToken cancellationToken = default);

    public ValueTask<IMinecraftChannel> BuildServerChannelAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default);
}