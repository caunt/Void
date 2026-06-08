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
public class ByteArgumentPassthroughSerializer : IArgumentSerializer
{
    public static IArgumentSerializer Instance { get; } = new ByteArgumentPassthroughSerializer();

    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return new BytePassthroughArgumentValue(this, buffer.ReadUnsignedByte());
    }

    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteUnsignedByte(value.As<BytePassthroughArgumentValue>().Value);
    }
}
