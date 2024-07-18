using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Network.Protocol.Services;

public interface IChannelBuilderService
{
    public ValueTask SearchChannelBuilderAsync(IPlayer player, CancellationToken cancellationToken = default);
    public ValueTask<IMinecraftChannel> BuildPlayerChannelAsync(IPlayer player, CancellationToken cancellationToken = default);
    public ValueTask<IMinecraftChannel> BuildServerChannelAsync(IServer server, CancellationToken cancellationToken = default);
}