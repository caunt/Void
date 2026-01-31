using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers.Passthrough;

public record TimePassthroughArgumentValue(IArgumentSerializer Serializer, int Value) : IPassthroughArgumentValue;
public class TimeArgumentPassthroughSerializer : IArgumentSerializer
{
    public static IArgumentSerializer Instance { get; } = new TimeArgumentPassthroughSerializer();

    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion < ProtocolVersion.MINECRAFT_1_19_4)
            return new TimePassthroughArgumentValue(this, 0);

        return new TimePassthroughArgumentValue(this, buffer.ReadInt());
    }

    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion < ProtocolVersion.MINECRAFT_1_19_4)
            return;

        buffer.WriteInt(value.As<TimePassthroughArgumentValue>().Value);
    }
}
