using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers.Passthrough;

public record BytePassthroughArgumentValue(IArgumentSerializer Serializer, byte Value) : IPassthroughArgumentValue;
public class ByteArgumentPassthroughSerializer : IArgumentSerializer
{
    public static IArgumentSerializer Instance { get; } = new ByteArgumentPassthroughSerializer();

    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return new BytePassthroughArgumentValue(this, buffer.ReadUnsignedByte());
    }

    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteUnsignedByte(value.As<BytePassthroughArgumentValue>().Value);
    }
}
