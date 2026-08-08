using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Messages.Packets;

// Do not use any of this anywhere except plugins

/// <summary>
/// Defines a Minecraft packet that can encode itself for a protocol version.
/// </summary>
public interface IMinecraftPacket : IMinecraftMessage
{
    /// <summary>
    /// Encodes the packet payload at the buffer's current position.
    /// </summary>
    /// <param name="buffer">The destination packet buffer.</param>
    /// <param name="protocolVersion">The protocol version whose wire layout is used.</param>
    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
}

/// <summary>
/// Defines a Minecraft packet type that can decode its payload from a buffer.
/// </summary>
/// <typeparam name="TSelf">The concrete packet type produced by decoding.</typeparam>
public interface IMinecraftPacket<out TSelf> : IMinecraftPacket where TSelf : IMinecraftPacket
{
    /// <summary>
    /// Decodes a packet payload at the buffer's current position.
    /// </summary>
    /// <param name="buffer">The source packet buffer.</param>
    /// <param name="protocolVersion">The protocol version whose wire layout is used.</param>
    /// <returns>The decoded packet instance.</returns>
    public static abstract TSelf Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion);
}
