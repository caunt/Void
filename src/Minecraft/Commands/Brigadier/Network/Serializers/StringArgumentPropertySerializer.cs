using System.IO;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Serializers;

public class StringArgumentPropertySerializer : IArgumentPropertySerializer<StringArgumentType>
{
    public static IArgumentPropertySerializer<StringArgumentType> Instance { get; } = new StringArgumentPropertySerializer();

    public StringArgumentType Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return buffer.ReadVarInt() switch
        {
            0 => StringArgumentType.Word(),
            1 => StringArgumentType.String(),
            2 => StringArgumentType.GreedyString(),
            var type => throw new InvalidDataException("Invalid string argument type " + type)
        };
    }

    public void Serialize(StringArgumentType value, BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        switch (value.Type)
        {
            case StringArgumentType.StringType.SingleWord:
                buffer.WriteVarInt(0);
                break;
            case StringArgumentType.StringType.QuotablePhrase:
                buffer.WriteVarInt(1);
                break;
            case StringArgumentType.StringType.GreedyPhrase:
                buffer.WriteVarInt(2);
                break;
            default:
                throw new InvalidDataException("Invalid string argument type " + value.Type);
        }
    }
}
