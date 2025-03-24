using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;
using Void.Proxy.Api.Network.IO.Messages.Packets;

namespace Void.Proxy.Api.Network.IO.Streams.Packet;

public delegate TPacket MinecraftPacketDecoder<out TPacket>(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion) where TPacket : IMinecraftPacket;