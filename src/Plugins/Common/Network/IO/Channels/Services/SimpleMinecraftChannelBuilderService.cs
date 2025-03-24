using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using Void.Proxy.Api.Events.Channels;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Network.IO.Channels.Services;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Network;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Transparent;

namespace Void.Proxy.Plugins.Common.Network.IO.Channels.Services;

public class SimpleMinecraftChannelBuilderService(ILogger<SimpleMinecraftChannelBuilderService> logger, IEventService events) : IChannelBuilderService
{
    public const int MaxHandshakeSize = 4096;

    private Memory<byte> _buffer = Memory<byte>.Empty;
    private ChannelBuilder? _builder;
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

        var searchProtocolCodec = new SearchChannelBuilderEvent(player, buffer.AsMemory(0, length));
        await events.ThrowAsync(searchProtocolCodec, cancellationToken);

        if (searchProtocolCodec.Result is not null)
        {
            _builder = searchProtocolCodec.Result;
        }
        else
        {
            _builder = FallbackBuilder;
            IsFallbackBuilder = true;
            logger.LogWarning("Channel builder not found for a {Player} player", player);
        }

        _buffer = searchProtocolCodec.Buffer;
        _executed = true;
    }

    public async ValueTask<IMinecraftChannel> BuildPlayerChannelAsync(IPlayer player, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Building channel for a {Player} player", player);
        var channel = await BuildChannelAsync(player, Side.Client, player.Client.GetStream(), cancellationToken);
        logger.LogTrace("Client {RemoteEndPoint} is using {ChannelTypeName} channel implementation", player.RemoteEndPoint, channel.GetType().Name);

        if (_buffer.Length <= 0)
            return channel;

        channel.PrependBuffer(_buffer);
        _buffer = Memory<byte>.Empty;

        return channel;
    }

    public async ValueTask<IMinecraftChannel> BuildServerChannelAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default)
    {
        logger.LogTrace("Building channel for a {Server} server", server);

        var client = server.CreateTcpClient();
        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

        var channel = await BuildChannelAsync(player, Side.Server, client.GetStream(), cancellationToken);
        logger.LogTrace("Server {Name} is using {ChannelTypeName} channel implementation", server.Name, channel.GetType().Name);

        return channel;
    }

    private async ValueTask<IMinecraftChannel> BuildChannelAsync(IPlayer player, Side side, NetworkStream stream, CancellationToken cancellationToken = default)
    {
        if (_builder is null)
            throw new InvalidOperationException($"{nameof(IChannelBuilderService)} is not found yet. Call {nameof(SearchChannelBuilderAsync)} before channel building.");

        var channel = await _builder(player, side, stream, cancellationToken);

        if (!channel.IsConfigured)
            channel.Add<MinecraftTransparentMessageStream>();

        return channel;
    }

    private static ValueTask<IMinecraftChannel> FallbackBuilder(IPlayer player, Side side, NetworkStream networkStream, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult<IMinecraftChannel>(new SimpleMinecraftChannel(new SimpleNetworkStream(networkStream)));
    }
}