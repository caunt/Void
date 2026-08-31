using Void.Minecraft.Network;
using Void.Proxy.Plugins.Common.Services.Entities;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Entities;

public sealed class EntityIdService : AbstractEntityIdService<LegacyEntityIdState>
{
    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return protocolVersion >= ProtocolVersion.MINECRAFT_1_8 && Plugin.SupportedVersions.Contains(protocolVersion);
    }
}
