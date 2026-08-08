using Void.Minecraft.Buffers;
using Void.Minecraft.Network.Messages.Packets;

namespace Void.Minecraft.Network.Registries.PacketId.Mappings;

/// <summary>
/// Decodes a Minecraft packet payload for a protocol version.
/// </summary>
/// <typeparam name="TPacket">The packet type returned by the decoder.</typeparam>
/// <param name="buffer">The source payload buffer, advanced as fields are decoded.</param>
/// <param name="protocolVersion">The protocol version whose field layout is used.</param>
/// <returns>The decoded packet.</returns>
public delegate TPacket MinecraftPacketDecoder<out TPacket>(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion) where TPacket : IMinecraftPacket;
