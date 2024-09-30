using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Network;
using Void.Proxy.Common.Network.IO.Streams.Compression;
using Void.Proxy.Common.Network.IO.Streams.Packet;
using Void.Proxy.Common.Services;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Services;

public class CompressionService(ILogger<CompressionService> logger) : IPluginService
{
    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Message is not SetCompressionPacket setCompression)
            return;

        @event.Link.ServerChannel.AddBefore<MinecraftPacketMessageStream, SharpZipLibCompressionMessageStream>();
        logger.LogTrace("Link {Link} enabled compression in server channel with threshold {CompressionThreshold}", @event.Link, setCompression.Threshold);

        var zlibStream = @event.Link.ServerChannel.Get<SharpZipLibCompressionMessageStream>();
        zlibStream.CompressionThreshold = setCompression.Threshold;
    }

    [Subscribe]
    public void OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Message is not SetCompressionPacket setCompression)
            return;

        @event.Link.PlayerChannel.AddBefore<MinecraftPacketMessageStream, SharpZipLibCompressionMessageStream>();
        logger.LogTrace("Link {Link} enabled compression in player channel with threshold {CompressionThreshold}", @event.Link, setCompression.Threshold);

        var zlibStream = @event.Link.PlayerChannel.Get<SharpZipLibCompressionMessageStream>();
        zlibStream.CompressionThreshold = setCompression.Threshold;

        @event.Link.PlayerChannel.Resume();
    }
}