using System.IO;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers;

/// <summary>Encodes string parsing modes as protocol VarInt discriminators.</summary>
public class StringArgumentSerializer : IArgumentSerializer
{
    /// <summary>Gets the shared stateless serializer.</summary>
    public static IArgumentSerializer Instance { get; } = new StringArgumentSerializer();

    /// <inheritdoc/>
    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return buffer.ReadVarInt() switch
        {
            0 => StringArgumentType.Word(),
            1 => StringArgumentType.String(),
            2 => StringArgumentType.GreedyString(),
            var type => throw new InvalidDataException("Invalid string argument type " + type)
        };
    }

    /// <inheritdoc/>
    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var stringArgumentType = value.As<StringArgumentType>();

        switch (stringArgumentType.Type)
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
                throw new InvalidDataException("Invalid string argument type " + stringArgumentType.Type);
        }
    }
}
