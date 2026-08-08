using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers.Passthrough;

/// <summary>
/// Represents a passthrough argument value whose serialized payload is a single unsigned byte.
/// </summary>
/// <param name="Serializer">The serializer that created and serializes this value.</param>
/// <param name="Value">The raw byte value preserved by the passthrough argument.</param>
public record BytePassthroughArgumentValue(IArgumentSerializer Serializer, byte Value) : IPassthroughArgumentValue;
/// <summary>Reads and writes an opaque one-byte argument payload.</summary>
public class ByteArgumentPassthroughSerializer : IArgumentSerializer
{
    /// <summary>Gets the shared stateless serializer.</summary>
    public static IArgumentSerializer Instance { get; } = new ByteArgumentPassthroughSerializer();

    /// <inheritdoc/>
    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return new BytePassthroughArgumentValue(this, buffer.ReadUnsignedByte());
    }

    /// <inheritdoc/>
    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteUnsignedByte(value.As<BytePassthroughArgumentValue>().Value);
    }
}
