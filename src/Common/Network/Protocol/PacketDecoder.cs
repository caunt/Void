using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.IO.Messages;

namespace Void.Proxy.Common.Network.Protocol;

public delegate TPacket PacketDecoder<out TPacket>(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion) where TPacket : IMinecraftPacket;