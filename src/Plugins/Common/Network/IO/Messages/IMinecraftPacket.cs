using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Network.IO.Buffers;
using Void.Proxy.API.Network.IO.Messages;

namespace Void.Proxy.Plugins.Common.Network.IO.Messages;

public interface IMinecraftPacket : IMinecraftMessage
{
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
}

public interface IMinecraftPacket<out TSelf> : IMinecraftPacket where TSelf : IMinecraftPacket
{
    public static abstract TSelf Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
}