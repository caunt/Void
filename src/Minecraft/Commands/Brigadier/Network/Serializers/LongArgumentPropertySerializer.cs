using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Serializers;

public class LongArgumentPropertySerializer : IArgumentPropertySerializer<LongArgumentType>
{
    public const byte HAS_MINIMUM = 0x01;
    public const byte HAS_MAXIMUM = 0x02;

    public static IArgumentPropertySerializer<LongArgumentType> Instance { get; } = new LongArgumentPropertySerializer();

    public LongArgumentType Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var flags = buffer.ReadUnsignedByte();

        var minimum = (flags & HAS_MINIMUM) != 0 ? buffer.ReadLong() : long.MinValue;
        var maximum = (flags & HAS_MAXIMUM) != 0 ? buffer.ReadLong() : long.MaxValue;

        return LongArgumentType.LongArgument(minimum, maximum);
    }

    public void Serialize(LongArgumentType value, BufferSpan buffer, ProtocolVersion protocolVersion)
    {
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
