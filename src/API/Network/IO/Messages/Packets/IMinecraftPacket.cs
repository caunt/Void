using Void.Proxy.Api.Mojang.Minecraft.Network;
using Void.Proxy.Api.Network.IO.Buffers;

namespace Void.Proxy.Api.Network.IO.Messages.Packets;

// Do not use any of this anywhere except plugins

public interface IMinecraftPacket : IMinecraftMessage
{
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
}

public interface IMinecraftPacket<out TSelf> : IMinecraftPacket where TSelf : IMinecraftPacket
{
    public static abstract TSelf Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
}
