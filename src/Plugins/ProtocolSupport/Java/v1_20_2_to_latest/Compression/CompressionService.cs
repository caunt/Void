﻿using Microsoft.Extensions.Logging;
using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Network.Protocol.Compression;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Compression;

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