using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages;

namespace Void.Proxy.API.Network.Protocol;

public delegate TSelf PacketDecoder<out TSelf>(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion) where TSelf : IMinecraftPacket;