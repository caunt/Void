using Microsoft.Extensions.Logging;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Compression;
using Void.Proxy.Plugins.Common.Network.IO.Streams.Packet;

namespace Void.Proxy.Plugins.Common.Services.Compression;

public abstract class AbstractCompressionService(ILogger<AbstractCompressionService> logger) : IPluginCommonService
{
    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        if (!IsCompressionPacket(@event.Message, out var threshold))
            return;

        @event.Link.ServerChannel.AddBefore<MinecraftPacketMessageStream, SharpZipLibCompressionMessageStream>();
        logger.LogTrace("Link {Link} enabled compression in server channel with threshold {CompressionThreshold}", @event.Link, threshold);

        var zlibStream = @event.Link.ServerChannel.Get<SharpZipLibCompressionMessageStream>();
        zlibStream.CompressionThreshold = threshold;
    }

    [Subscribe(PostOrder.First)]
    public void OnMessageSent(MessageSentEvent @event)
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
