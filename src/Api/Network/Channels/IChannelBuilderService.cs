using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Network.Channels;

public interface IChannelBuilderService
{
    public bool IsFallbackBuilder { get; }

    public ValueTask SearchChannelBuilderAsync(IPlayer player, CancellationToken cancellationToken = default);

    public ValueTask<INetworkChannel> BuildPlayerChannelAsync(IPlayer player, CancellationToken cancellationToken = default);

    public ValueTask<INetworkChannel> BuildServerChannelAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default);
}
