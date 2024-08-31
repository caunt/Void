using System.Net.Sockets;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Network.Protocol.Services;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.Players;

public class Player(AsyncServiceScope scope, TcpClient client) : IPlayer
{
    private IMinecraftChannel? _channel;

    public AsyncServiceScope Scope => scope;
    public TcpClient Client => client;
    public string RemoteEndPoint { get; } = client.Client.RemoteEndPoint?.ToString() ?? "Unknown?";

    public string? Name { get; set; }
    public string? Brand { get; set; }

    public ProtocolVersion ProtocolVersion { get; set; } = ProtocolVersion.Oldest; // we do not know Player protocol version yet, use the oldest possible

    public async ValueTask<IMinecraftChannel> BuildServerChannelAsync(IServer server, CancellationToken cancellationToken = default)
    {
        var channelBuilder = await GetChannelBuilderAsync(cancellationToken);
        return await channelBuilder.BuildServerChannelAsync(server, cancellationToken);
    }

    public async ValueTask<IMinecraftChannel> GetChannelAsync(CancellationToken cancellationToken = default)
    {
        if (_channel is not null)
            return _channel;

        var channelBuilder = await GetChannelBuilderAsync(cancellationToken);
        _channel = await channelBuilder.BuildPlayerChannelAsync(this, cancellationToken);

        return _channel;
    }

    public async ValueTask DisposeAsync()
    {
        await scope.DisposeAsync();
    }

    public override string ToString()
    {
        return Name ?? RemoteEndPoint;
    }

    private async ValueTask<IChannelBuilderService> GetChannelBuilderAsync(CancellationToken cancellationToken = default)
    {
        var channelBuilder = scope.ServiceProvider.GetRequiredService<IChannelBuilderService>();
        await channelBuilder.SearchChannelBuilderAsync(this, cancellationToken);

        return channelBuilder;
    }
}