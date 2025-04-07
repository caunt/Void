using Void.Common.Network.Channels;
using Void.Common.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Network.IO.Channels;

public interface IChannelBuilderService
{
    public bool IsFallbackBuilder { get; }

    public ValueTask SearchChannelBuilderAsync(IPlayer player, CancellationToken cancellationToken = default);

    public ValueTask<INetworkChannel> BuildPlayerChannelAsync(IPlayer player, CancellationToken cancellationToken = default);

    public ValueTask<INetworkChannel> BuildServerChannelAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default);
}
