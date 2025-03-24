using Microsoft.Extensions.Logging;
using Void.Proxy.Api.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Services.Compression;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Compression;

public class CompressionService(ILogger<CompressionService> logger) : AbstractCompressionService(logger)
{
    protected override bool IsCompressionPacket(IMinecraftMessage message, out int threshold)
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