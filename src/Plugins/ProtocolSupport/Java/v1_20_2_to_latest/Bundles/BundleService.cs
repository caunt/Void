﻿using Void.Proxy.API.Network.IO.Messages;
using Void.Proxy.Plugins.Common.Services.Bundles;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Bundles;

public class BundleService : AbstractBundleService
{
    protected override bool IsBundlePacket(IMinecraftMessage message)
    {
        return message is BundleDelimiterPacket;
    }
}