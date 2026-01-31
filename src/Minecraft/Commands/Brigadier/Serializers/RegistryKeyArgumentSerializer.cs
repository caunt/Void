using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers;

public class RegistryKeyArgumentSerializer : IArgumentSerializer
{
    public static IArgumentSerializer Instance { get; } = new RegistryKeyArgumentSerializer();

    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return new RegistryKeyArgumentType(Identifier: buffer.ReadString());
    }

    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(value.As<RegistryKeyArgumentType>().Identifier);
    }
}
