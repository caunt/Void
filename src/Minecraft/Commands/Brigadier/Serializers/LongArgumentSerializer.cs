using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers;

public class LongArgumentSerializer : IArgumentSerializer
{
    public const byte HAS_MINIMUM = 0x01;
    public const byte HAS_MAXIMUM = 0x02;

    public static IArgumentSerializer Instance { get; } = new LongArgumentSerializer();

    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var flags = buffer.ReadUnsignedByte();

        var minimum = (flags & HAS_MINIMUM) != 0 ? buffer.ReadLong() : long.MinValue;
        var maximum = (flags & HAS_MAXIMUM) != 0 ? buffer.ReadLong() : long.MaxValue;

        return LongArgumentType.LongArgument(minimum, maximum);
    }

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
