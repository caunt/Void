using Void.Minecraft.Network;

namespace Void.Proxy.Plugins.Common.Network.Packets.Transformations.v1_7_6_to_v1_8;

public abstract record BaseTransformation1_8() : PacketTransformation(ProtocolVersion.MINECRAFT_1_7_6, ProtocolVersion.MINECRAFT_1_8);
