using System.Net.Sockets;
using Void.Proxy.API.Events.Handshake;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Streams;
using Void.Proxy.API.Network.IO.Streams.Transparent;
using Void.Proxy.API.Network.Protocol.Services;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.Network.Protocol.Services;

public class ChannelBuilderService(
    ILogger<ChannelBuilderService> logger,
    IEventService events) : IChannelBuilderService
{
    public const int MaxHandshakeSize = 4096;
    private Memory<byte> _buffer = Memory<byte>.Empty;

    private Func<NetworkStream, Task<IMinecraftChannel>> _builder = networkStream => Task.FromResult<IMinecraftChannel>(new SimpleMinecraftChannel(new SimpleNetworkStream(networkStream)));
    private bool _found;

    public async ValueTask SearchChannelBuilderAsync(IPlayer player, CancellationToken cancellationToken = default)
    {
        if (_found)
            return;

        var stream = player.Client.GetStream();
        var buffer = new byte[MaxHandshakeSize];
        var length = await stream.ReadAsync(buffer, cancellationToken);

        var searchProtocolCodec = new CreateChannelBuilderEvent { Buffer = buffer[..length] };
        await events.ThrowAsync(searchProtocolCodec, cancellationToken);

        if (searchProtocolCodec.Result is not null)
            _builder = searchProtocolCodec.Result;

        _buffer = searchProtocolCodec.Buffer;
        _found = true;
    }

    public async ValueTask<IMinecraftChannel> BuildPlayerChannelAsync(IPlayer player, CancellationToken cancellationToken = default)
    {
        var channel = await BuildChannelAsync(player.Client.GetStream(), cancellationToken);
        logger.LogDebug("Client {RemoteEndPoint} is using {ChannelTypeName} channel implementation", player.RemoteEndPoint, channel.GetType()
            .Name);

        if (_buffer.Length <= 0)
            return channel;

        channel.PrependBuffer(_buffer);
        _buffer = Memory<byte>.Empty;

        return channel;
    }

    public async ValueTask<IMinecraftChannel> BuildServerChannelAsync(IServer server, CancellationToken cancellationToken = default)
    {
        var channel = await BuildChannelAsync(server.CreateTcpClient()
            .GetStream(), cancellationToken);
        logger.LogDebug("Server {Name} is using {ChannelTypeName} channel implementation", server.Name, channel.GetType()
            .Name);

        return channel;
    }

    private async ValueTask<IMinecraftChannel> BuildChannelAsync(NetworkStream stream, CancellationToken cancellationToken = default)
    {
        var channel = await _builder(stream);

        if (!channel.IsConfigured)
            channel.Add<MinecraftTransparentMessageStream>();

        return channel;
    }
}