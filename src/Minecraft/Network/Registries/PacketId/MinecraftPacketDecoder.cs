using Void.Minecraft.Buffers;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Minecraft.Network.Registries.PacketId;

public delegate TPacket MinecraftPacketDecoder<out TPacket>(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion) where TPacket : IMinecraftPacket;
