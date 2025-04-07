using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Network.IO.Channels.Services;

public interface IChannelBuilderService
{
    public bool IsFallbackBuilder { get; }

    public ValueTask SearchChannelBuilderAsync(IPlayer player, CancellationToken cancellationToken = default);

    public ValueTask<IMinecraftChannel> BuildPlayerChannelAsync(IPlayer player, CancellationToken cancellationToken = default);

    public ValueTask<IMinecraftChannel> BuildServerChannelAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default);
}
