using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Serializers;

public class TimeArgumentSerializer : IArgumentPropertySerializer<int>
{
    public static IArgumentPropertySerializer<int> Instance { get; } = new TimeArgumentSerializer();

    public int Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion < ProtocolVersion.MINECRAFT_1_19_4)
            return 0;

        return buffer.ReadInt();
    }

    public void Serialize(int value, BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion < ProtocolVersion.MINECRAFT_1_19_4)
            return;

        buffer.WriteInt(value);
    }
}
