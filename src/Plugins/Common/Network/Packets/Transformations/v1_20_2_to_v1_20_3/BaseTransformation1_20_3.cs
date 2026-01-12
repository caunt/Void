using Void.Minecraft.Network;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_20_2_to_v1_20_3;

public abstract record BaseTransformation1_20_3() : PacketTransformation(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.MINECRAFT_1_20_3);
