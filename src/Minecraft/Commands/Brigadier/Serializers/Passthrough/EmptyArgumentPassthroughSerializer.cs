using Void.Minecraft.Buffers;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers.Passthrough;

/// <summary>Represents an opaque argument declaration with no property bytes.</summary>
/// <param name="Serializer">The serializer that created the value.</param>
public record EmptyPassthroughArgumentValue(IArgumentSerializer Serializer) : IPassthroughArgumentValue;
/// <summary>Creates and writes passthrough argument values with empty payloads.</summary>
public class EmptyArgumentPassthroughSerializer : IArgumentSerializer
{
    /// <summary>Gets the shared stateless serializer.</summary>
    public static IArgumentSerializer Instance { get; } = new EmptyArgumentPassthroughSerializer();

    /// <inheritdoc/>
    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return new EmptyPassthroughArgumentValue(this);
    }

    /// <inheritdoc/>
    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        // No data to serialize
    }
}
