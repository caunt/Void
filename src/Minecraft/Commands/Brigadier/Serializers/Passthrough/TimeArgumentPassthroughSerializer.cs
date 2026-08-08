using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers.Passthrough;

/// <summary>Preserves the minimum time value introduced in protocol 1.19.4.</summary>
/// <param name="Serializer">The serializer that created the value.</param>
/// <param name="Value">The raw minimum value, or zero for older protocols.</param>
public record TimePassthroughArgumentValue(IArgumentSerializer Serializer, int Value) : IPassthroughArgumentValue;
/// <summary>Reads and writes the time-argument integer payload used since protocol 1.19.4.</summary>
public class TimeArgumentPassthroughSerializer : IArgumentSerializer
{
    /// <summary>Gets the shared stateless serializer.</summary>
    public static IArgumentSerializer Instance { get; } = new TimeArgumentPassthroughSerializer();

    /// <inheritdoc/>
    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion < ProtocolVersion.MINECRAFT_1_19_4)
            return new TimePassthroughArgumentValue(this, 0);

        return new TimePassthroughArgumentValue(this, buffer.ReadInt());
    }

    /// <inheritdoc/>
    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion < ProtocolVersion.MINECRAFT_1_19_4)
            return;

        buffer.WriteInt(value.As<TimePassthroughArgumentValue>().Value);
    }
}
