using Void.Minecraft.Buffers;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers.Passthrough;

public record EmptyPassthroughArgumentValue(IArgumentSerializer Serializer) : IPassthroughArgumentValue;
public class EmptyArgumentPassthroughSerializer : IArgumentSerializer
{
    public static IArgumentSerializer Instance { get; } = new EmptyArgumentPassthroughSerializer();

    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return new EmptyPassthroughArgumentValue(this);
    }

    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        // No data to serialize
    }
}
