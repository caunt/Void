using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers;

/// <summary>Preserves an opaque variable-length registry identifier.</summary>
/// <param name="Serializer">The serializer that created the value.</param>
/// <param name="Value">The raw registry identifier.</param>
public record RegistryIdPassthroughArgumentValue(IArgumentSerializer Serializer, int Value) : IPassthroughArgumentValue;
/// <summary>Reads and writes passthrough registry identifiers as VarInts.</summary>
public class RegistryIdArgumentSerializer : IArgumentSerializer
{
    /// <summary>Gets the shared stateless serializer.</summary>
    public static IArgumentSerializer Instance { get; } = new RegistryIdArgumentSerializer();

    /// <inheritdoc/>
    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return new RegistryIdPassthroughArgumentValue(this, buffer.ReadVarInt());
    }

    /// <inheritdoc/>
    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(value.As<RegistryIdPassthroughArgumentValue>().Value);
    }
}
