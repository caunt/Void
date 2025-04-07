using Microsoft.Extensions.Logging;
using Void.Common;
using Void.Proxy.Plugins.Common.Services.Compression;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Compression;

public class CompressionService(ILogger<CompressionService> logger) : AbstractCompressionService(logger)
{
    protected override bool IsCompressionPacket(INetworkMessage message, out int threshold)
    {
        if (message is SetCompressionPacket setCompression)
        {
            threshold = setCompression.Threshold;
            return true;
        }

        threshold = 0;
        return false;
    }
}
