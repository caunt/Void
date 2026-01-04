using Void.Minecraft.Network;
using Void.Proxy.Plugins.Common.Services.Transformations;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Transformations;

public class TransformationService : AbstractTransformationService
{
    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return Plugin.SupportedVersions.Contains(protocolVersion);
    }
}
