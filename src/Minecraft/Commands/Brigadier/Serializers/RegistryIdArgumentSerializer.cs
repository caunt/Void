using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers;

public record RegistryIdPassthroughArgumentValue(IArgumentSerializer Serializer, int Value) : IPassthroughArgumentValue;
public class RegistryIdArgumentSerializer : IArgumentSerializer
{
    public static IArgumentSerializer Instance { get; } = new RegistryIdArgumentSerializer();

    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return new RegistryIdPassthroughArgumentValue(this, buffer.ReadVarInt());
    }

    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(value.As<RegistryIdPassthroughArgumentValue>().Value);
    }
}
