using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Compression;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Packet;
using Void.Proxy.Plugins.Common.Services;

namespace Void.Proxy.Plugins.Common.Network.Protocol.Compression;

public abstract class AbstractCompressionService(ILogger<AbstractCompressionService> logger) : IPluginService
{
    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        if (!IsCompressionPacket(@event.Message, out var threshold))
            return;

        @event.Link.ServerChannel.AddBefore<MinecraftPacketMessageStream, SharpZipLibCompressionMessageStream>();
        logger.LogTrace("Link {Link} enabled compression in server channel with threshold {CompressionThreshold}", @event.Link, threshold);

        var zlibStream = @event.Link.ServerChannel.Get<SharpZipLibCompressionMessageStream>();
        zlibStream.CompressionThreshold = threshold;
    }

    [Subscribe(PostOrder.First)]
    public void OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        if (!IsCompressionPacket(@event.Message, out var threshold))
            return;

        @event.Link.PlayerChannel.AddBefore<MinecraftPacketMessageStream, SharpZipLibCompressionMessageStream>();
        logger.LogTrace("Link {Link} enabled compression in player channel with threshold {CompressionThreshold}", @event.Link, threshold);

        var zlibStream = @event.Link.PlayerChannel.Get<SharpZipLibCompressionMessageStream>();
        zlibStream.CompressionThreshold = threshold;
    }

    protected abstract bool IsCompressionPacket(IMinecraftMessage message, out int threshold);
}