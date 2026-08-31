using Void.Minecraft.Network;
using Void.Proxy.Plugins.Common.Services.Entities;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_13_to_1_20_1.Entities;

public sealed class EntityIdService : AbstractEntityIdService<LegacyEntityIdState>
{
    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return protocolVersion >= ProtocolVersion.MINECRAFT_1_13 && protocolVersion <= ProtocolVersion.MINECRAFT_1_15_2;
    }
}
