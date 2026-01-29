using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Serializers;

public class BoolArgumentSerializer : IArgumentPropertySerializer<bool>
{
    public static IArgumentPropertySerializer<bool> Instance { get; } = new BoolArgumentSerializer();

    public bool Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return buffer.ReadBoolean();
    }

    public void Serialize(bool value, BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteBoolean(value);
    }
}
