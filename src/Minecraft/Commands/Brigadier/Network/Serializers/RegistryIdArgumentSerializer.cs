using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Serializers;

public class RegistryIdArgumentSerializer : IArgumentPropertySerializer<int>
{
    public static IArgumentPropertySerializer<int> Instance { get; } = new RegistryIdArgumentSerializer();

    public int Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return buffer.ReadVarInt();
    }

    public void Serialize(int value, BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(value);
    }
}
