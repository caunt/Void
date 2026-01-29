using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Serializers;

public class FloatArgumentPropertySerializer : IArgumentPropertySerializer<FloatArgumentType>
{
    public const byte HAS_MINIMUM = 0x01;
    public const byte HAS_MAXIMUM = 0x02;

    public static IArgumentPropertySerializer<FloatArgumentType> Instance { get; } = new FloatArgumentPropertySerializer();

    public FloatArgumentType Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var flags = buffer.ReadUnsignedByte();

        var minimum = (flags & HAS_MINIMUM) != 0 ? buffer.ReadFloat() : float.MinValue;
        var maximum = (flags & HAS_MAXIMUM) != 0 ? buffer.ReadFloat() : float.MaxValue;

        return FloatArgumentType.FloatArgument(minimum, maximum);
    }

    public void Serialize(FloatArgumentType value, BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var hasMinimum = value.Minimum != float.MinValue;
        var hasMaximum = value.Maximum != float.MaxValue;

        var flag = (byte)((hasMinimum ? HAS_MINIMUM : 0) | (hasMaximum ? HAS_MAXIMUM : 0));

        buffer.WriteUnsignedByte(flag);

        if (hasMinimum)
            buffer.WriteFloat(value.Minimum);

        if (hasMaximum)
            buffer.WriteFloat(value.Maximum);
    }
}
