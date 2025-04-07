using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Messages.Packets;

// Do not use any of this anywhere except plugins

public interface IMinecraftPacket : IMinecraftMessage
{
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
}

public interface IMinecraftPacket<out TSelf> : IMinecraftPacket where TSelf : IMinecraftPacket
{
    public static abstract TSelf Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
}
