using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Serializers;

public class IntegerArgumentPropertySerializer : IArgumentPropertySerializer<IntegerArgumentType>
{
    public const byte HAS_MINIMUM = 0x01;
    public const byte HAS_MAXIMUM = 0x02;

    public static IArgumentPropertySerializer<IntegerArgumentType> Instance { get; } = new IntegerArgumentPropertySerializer();

    public IntegerArgumentType Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var flags = buffer.ReadUnsignedByte();

        var minimum = (flags & HAS_MINIMUM) != 0 ? buffer.ReadInt() : int.MinValue;
        var maximum = (flags & HAS_MAXIMUM) != 0 ? buffer.ReadInt() : int.MaxValue;

        return IntegerArgumentType.IntegerArgument(minimum, maximum);
    }

    public void Serialize(IntegerArgumentType value, BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var hasMinimum = value.Minimum != int.MinValue;
        var hasMaximum = value.Maximum != int.MaxValue;

        var flag = (byte)((hasMinimum ? HAS_MINIMUM : 0) | (hasMaximum ? HAS_MAXIMUM : 0));

        buffer.WriteUnsignedByte(flag);

        if (hasMinimum)
            buffer.WriteInt(value.Minimum);

        if (hasMaximum)
            buffer.WriteInt(value.Maximum);
    }
}
