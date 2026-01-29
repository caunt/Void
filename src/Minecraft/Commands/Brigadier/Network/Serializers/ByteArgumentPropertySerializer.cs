using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Serializers;

public class ByteArgumentPropertySerializer : IArgumentPropertySerializer<byte>
{
    public static IArgumentPropertySerializer<byte> Instance { get; } = new ByteArgumentPropertySerializer();

    public byte Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return buffer.ReadUnsignedByte();
    }

    public void Serialize(byte value, BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteUnsignedByte(value);
    }
}
