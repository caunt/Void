using Void.Minecraft.Network;
using Void.Proxy.Plugins.Common.Services.Transformations;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Transformations;

public class TransformationService : AbstractTransformationService
{
    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return Plugin.SupportedVersions.Contains(protocolVersion);
    }
}
