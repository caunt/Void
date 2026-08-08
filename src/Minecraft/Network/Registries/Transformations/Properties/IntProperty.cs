using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>
/// Represents the encoded payload of a fixed-width integer packet property.
/// </summary>
/// <param name="Value">The bytes containing the big-endian Minecraft binary representation of the integer.</param>
/// <remarks>
/// The supplied memory is stored without validation. It must contain at least four bytes before the value is decoded or written.
/// </remarks>
public record IntProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<IntProperty>
{
    public int AsPrimitive => new MinecraftBuffer(Value.Span).ReadInt();

    public static IntProperty FromPrimitive(int value)
    {
        Span<byte> span = stackalloc byte[4];
        var buffer = new MinecraftBuffer(span);
        buffer.WriteInt(value);

        return new IntProperty(span.ToArray());
    }

    public static IntProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadInt());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteInt(AsPrimitive);
    }
}
