using Void.Common;
using Void.Proxy.Plugins.Common.Services.Bundles;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Packets.Clientbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Bundles;

public class BundleService : AbstractBundleService
{
    protected override bool IsBundlePacket(INetworkMessage message)
    {
        return message is BundleDelimiterPacket;
    }
}
