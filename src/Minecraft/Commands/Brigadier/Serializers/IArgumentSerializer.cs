using Void.Minecraft.Buffers;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers;

public interface IArgumentSerializer
{
    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion);
    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion);
}
