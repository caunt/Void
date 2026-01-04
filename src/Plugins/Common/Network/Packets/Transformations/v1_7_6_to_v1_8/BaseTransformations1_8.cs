using Void.Minecraft.Network;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_7_6_to_v1_8;

public abstract record BaseTransformations1_8() : BaseTransformations(ProtocolVersion.MINECRAFT_1_7_6, ProtocolVersion.MINECRAFT_1_8);
