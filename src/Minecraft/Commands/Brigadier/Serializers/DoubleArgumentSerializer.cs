using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers;

public class DoubleArgumentSerializer : IArgumentSerializer
{
    /// <summary>
    /// Indicates that a serialized <see cref="DoubleArgumentType"/> includes an explicit minimum bound.
    /// </summary>
    public const byte HAS_MINIMUM = 0x01;
    public const byte HAS_MAXIMUM = 0x02;

    public static IArgumentSerializer Instance { get; } = new DoubleArgumentSerializer();

    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var flags = buffer.ReadUnsignedByte();

        var minimum = (flags & HAS_MINIMUM) != 0 ? buffer.ReadDouble() : double.MinValue;
        var maximum = (flags & HAS_MAXIMUM) != 0 ? buffer.ReadDouble() : double.MaxValue;

        return DoubleArgumentType.DoubleArgument(minimum, maximum);
    }

    /// <summary>
    /// Serializes a double command argument into the Brigadier argument payload.
    /// </summary>
    /// <param name="argumentType">The <see cref="DoubleArgumentType"/> instance to serialize.</param>
    /// <param name="buffer">The destination buffer that receives the flag byte and any explicit bounds.</param>
    /// <param name="protocolVersion">The protocol version for the serialized payload; this serializer uses the same format for every version.</param>
    /// <exception cref="System.InvalidCastException">Thrown when <paramref name="argumentType"/> is not a <see cref="DoubleArgumentType"/>.</exception>
    public void Serialize(IArgumentType argumentType, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var value = argumentType.As<DoubleArgumentType>();

        var hasMinimum = value.Minimum != double.MinValue;
        var hasMaximum = value.Maximum != double.MaxValue;

        var flag = (byte)((hasMinimum ? HAS_MINIMUM : 0) | (hasMaximum ? HAS_MAXIMUM : 0));

        buffer.WriteUnsignedByte(flag);

        if (hasMinimum)
            buffer.WriteDouble(value.Minimum);

        if (hasMaximum)
            buffer.WriteDouble(value.Maximum);
    }
}
