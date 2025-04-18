using Void.Proxy.Api.Network.Messages;
using Void.Proxy.Plugins.Common.Services.Bundles;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Bundles;

public class BundleService : AbstractBundleService
{
    protected override bool IsBundlePacket(INetworkMessage message)
    {
        return message is BundleDelimiterPacket;
    }
}
