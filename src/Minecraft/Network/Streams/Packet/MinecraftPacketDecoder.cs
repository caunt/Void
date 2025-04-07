using Void.Minecraft.Buffers;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Minecraft.Network.Streams.Packet;

public delegate TPacket MinecraftPacketDecoder<out TPacket>(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion) where TPacket : IMinecraftPacket;
