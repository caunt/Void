using Void.Minecraft.Buffers;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Serializers;

public interface IArgumentPropertySerializer<T>
{
    public static abstract IArgumentPropertySerializer<T> Instance { get; }

    public T? Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion);

    public void Serialize(T value, BufferSpan buffer, ProtocolVersion protocolVersion);
}
