﻿using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Network.Protocol.Bundles;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Bundles;

public class BundleService : AbstractBundleService
{
    protected override bool IsBundlePacket(IMinecraftMessage message)
    {
        return message is BundleDelimiterPacket;
    }
}