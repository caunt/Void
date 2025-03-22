using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.Plugins.Common.Network.IO.Messages;

namespace Void.Proxy.Plugins.Common.Network.Protocol.Packets;

public delegate TPacket PacketDecoder<out TPacket>(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion) where TPacket : IMinecraftPacket;