using Void.Minecraft.Buffers;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Serializers;

public class EmptyArgumentPropertySerializer : IArgumentPropertySerializer<object?>
{
    public static IArgumentPropertySerializer<object?> Instance { get; } = new EmptyArgumentPropertySerializer();

    public object? Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return null;
    }

    public void Serialize(object? value, BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        // No data to serialize
    }
}
