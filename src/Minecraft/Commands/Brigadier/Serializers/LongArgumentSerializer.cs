using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers;

/// <summary>Reads and writes optional long-argument bounds.</summary>
public class LongArgumentSerializer : IArgumentSerializer
{
    /// <summary>Indicates that an explicit minimum follows the flag byte.</summary>
    public const byte HAS_MINIMUM = 0x01;
    /// <summary>Indicates that an explicit maximum follows the flag byte.</summary>
    public const byte HAS_MAXIMUM = 0x02;

    /// <summary>Gets the shared stateless serializer.</summary>
    public static IArgumentSerializer Instance { get; } = new LongArgumentSerializer();

    /// <inheritdoc/>
    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var flags = buffer.ReadUnsignedByte();

        var minimum = (flags & HAS_MINIMUM) != 0 ? buffer.ReadLong() : long.MinValue;
        var maximum = (flags & HAS_MAXIMUM) != 0 ? buffer.ReadLong() : long.MaxValue;

        return LongArgumentType.LongArgument(minimum, maximum);
    }

    /// <inheritdoc/>
    public void Serialize(IArgumentType argumentType, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var value = argumentType.As<LongArgumentType>();

        var hasMinimum = value.Minimum != long.MinValue;
        var hasMaximum = value.Maximum != long.MaxValue;

        var flag = (byte)((hasMinimum ? HAS_MINIMUM : 0) | (hasMaximum ? HAS_MAXIMUM : 0));

        buffer.WriteUnsignedByte(flag);

        if (hasMinimum)
            buffer.WriteLong(value.Minimum);

        if (hasMaximum)
            buffer.WriteLong(value.Maximum);
    }
}
