using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events.Channels;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Network.IO.Channels.Services;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;
using Void.Proxy.Common.Network.IO.Streams.Network;
using Void.Proxy.Common.Network.IO.Streams.Transparent;

namespace Void.Proxy.Common.Network.IO.Channels.Services;

public class SimpleChannelBuilderService(ILogger<SimpleChannelBuilderService> logger, IEventService events) : IMinecraftChannelBuilderService
{
    public const int MaxHandshakeSize = 4096;

    private Memory<byte> _buffer = Memory<byte>.Empty;
    private ChannelBuilder _builder = FallbackBuilder;
    private bool _executed;

    public bool IsFallbackBuilder { get; private set; }

    public async ValueTask SearchChannelBuilderAsync(IPlayer player, CancellationToken cancellationToken = default)
    {
        if (_executed)
            return;

        logger.LogTrace("Searching for channel builder for a {Player} player", player);

        var stream = player.Client.GetStream();
        var buffer = new byte[MaxHandshakeSize];
        var length = await stream.ReadAsync(buffer, cancellationToken);

        var searchProtocolCodec = new SearchChannelBuilderEvent { Player = player, Buffer = buffer[..length] };
        await events.ThrowAsync(searchProtocolCodec, cancellationToken);

        if (searchProtocolCodec.Result is not null)
        {
            _builder = searchProtocolCodec.Result;
        }
        else
        {
            IsFallbackBuilder = true;
            logger.LogWarning("Channel builder not found for a {Player} player", player);
        }

        _buffer = searchProtocolCodec.Buffer;
        _executed = true;
    }

    public async ValueTask<IMinecraftChannel> BuildPlayerChannelAsync(IPlayer player, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Building channel for a {Player} player", player);
        var channel = await BuildChannelAsync(Direction.Serverbound, player.Client.GetStream(), cancellationToken);
        logger.LogTrace("Client {RemoteEndPoint} is using {ChannelTypeName} channel implementation", player.RemoteEndPoint, channel.GetType().Name);

        if (_buffer.Length <= 0)
            return channel;

        channel.PrependBuffer(_buffer);
        _buffer = Memory<byte>.Empty;

        return channel;
    }

    public async ValueTask<IMinecraftChannel> BuildServerChannelAsync(IServer server, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Building channel for a {Server} server", server);

        var client = server.CreateTcpClient();
        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

        var channel = await BuildChannelAsync(Direction.Clientbound, client.GetStream(), cancellationToken);
        logger.LogTrace("Server {Name} is using {ChannelTypeName} channel implementation", server.Name, channel.GetType().Name);

        return channel;
    }

    private async ValueTask<IMinecraftChannel> BuildChannelAsync(Direction direction, NetworkStream stream, CancellationToken cancellationToken = default)
    {
        var channel = await _builder(direction, stream, cancellationToken);

        if (!channel.IsConfigured)
            channel.Add<MinecraftTransparentMessageStream>();

        return channel;
    }

    private static ValueTask<IMinecraftChannel> FallbackBuilder(Direction direction, NetworkStream networkStream, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult<IMinecraftChannel>(new SimpleChannel(new SimpleNetworkStream(networkStream)));
    }
}