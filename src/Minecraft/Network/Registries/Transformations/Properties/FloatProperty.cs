using System;
using Void.Minecraft.Buffers;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

public record FloatProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<FloatProperty>
{
    public float AsPrimitive => new MinecraftBuffer(Value.Span).ReadFloat();

    /// <summary>
    /// Creates a property payload containing the big-endian Minecraft binary representation of a single-precision floating-point value.
    /// </summary>
    /// <param name="value">The <see cref="float"/> value to serialize into the property payload.</param>
    /// <returns>A <see cref="FloatProperty"/> whose <see cref="Value"/> contains the serialized value.</returns>
    public static FloatProperty FromPrimitive(float value)
    {
        var bytes = new byte[4];
        var buffer = new MinecraftBuffer(bytes);
        buffer.WriteFloat(value);

        return new FloatProperty(bytes);
    }

    public static FloatProperty Read(ref MinecraftBuffer buffer)
    {
        return FromPrimitive(buffer.ReadFloat());
    }

    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteFloat(AsPrimitive);
    }
}
