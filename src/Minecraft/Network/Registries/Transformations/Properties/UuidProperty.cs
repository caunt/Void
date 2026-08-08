using System;
using Void.Minecraft.Buffers;
using Void.Minecraft.Profiles;

namespace Void.Minecraft.Network.Registries.Transformations.Properties;

/// <summary>
/// Represents a UUID encoded as two 64-bit Minecraft protocol values.
/// </summary>
/// <param name="Value">The encoded 16-byte value, retained without copying or validation.</param>
public record UuidProperty(ReadOnlyMemory<byte> Value) : IPacketProperty<UuidProperty>
{
    /// <summary>Gets a property containing the zero UUID.</summary>
    public static UuidProperty Empty { get; } = FromUuid(Uuid.Empty);

    /// <summary>Gets the UUID decoded from the property bytes.</summary>
    public Uuid AsUuid => new MinecraftBuffer(Value.Span).ReadUuid();

    /// <summary>Encodes a UUID as a packet property.</summary>
    /// <param name="value">The UUID to encode.</param>
    /// <returns>A property backed by a new 16-byte array.</returns>
    public static UuidProperty FromUuid(Uuid value)
    {
        var bytes = new byte[16];
        var buffer = new MinecraftBuffer(bytes.AsSpan());
        buffer.WriteUuid(value);

        return new UuidProperty(bytes);
    }

    /// <summary>
    /// Reads a UUID from the current position of the specified buffer and wraps its 16-byte representation in a property.
    /// </summary>
    /// <param name="buffer">The buffer to read from. Its position is advanced by 16 bytes.</param>
    /// <returns>A property containing the UUID read from <paramref name="buffer"/>.</returns>
    public static UuidProperty Read(ref MinecraftBuffer buffer)
    {
        return FromUuid(buffer.ReadUuid());
    }

    /// <summary>Writes the decoded UUID to a buffer.</summary>
    /// <param name="buffer">The destination buffer.</param>
    public void Write(ref MinecraftBuffer buffer)
    {
        buffer.WriteUuid(AsUuid);
    }
}
