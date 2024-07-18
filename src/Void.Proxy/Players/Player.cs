using System.Net.Sockets;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Network.Protocol.Services;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.Players;

public class Player(
    AsyncServiceScope scope,
    TcpClient client,
    string remoteEndPoint) : IPlayer
{
    private IMinecraftChannel? _channel;
    public AsyncServiceScope Scope => scope;
    public TcpClient Client => client;
    public string RemoteEndPoint => remoteEndPoint;

    public string? Name { get; set; }
    public string? Brand { get; set; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public ClientType ClientType { get; set; }

    public async ValueTask<IMinecraftChannel> BuildServerChannelAsync(IServer server)
    {
        var channelBuilder = await GetChannelBuilderAsync();
        return await channelBuilder.BuildServerChannelAsync(server);
    }

    public async ValueTask<IMinecraftChannel> GetChannelAsync()
    {
        if (_channel is not null)
            return _channel;

        var channelBuilder = await GetChannelBuilderAsync();
        _channel = await channelBuilder.BuildPlayerChannelAsync(this);

        return _channel;
    }

    public async ValueTask DisposeAsync()
    {
        await scope.DisposeAsync();
    }

    public override string ToString()
    {
        return Name ?? remoteEndPoint;
    }

    private async ValueTask<IChannelBuilderService> GetChannelBuilderAsync()
    {
        var channelBuilder = scope.ServiceProvider.GetRequiredService<IChannelBuilderService>();
        await channelBuilder.SearchChannelBuilderAsync(this);

        return channelBuilder;
    }
}