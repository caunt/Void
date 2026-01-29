using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.Network.Arguments;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Serializers;

public class RegistryKeyArgumentSerializer : IArgumentPropertySerializer<RegistryKeyArgument>
{
    public static IArgumentPropertySerializer<RegistryKeyArgument> Instance { get; } = new RegistryKeyArgumentSerializer();

    public RegistryKeyArgument Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return new RegistryKeyArgument(identifier: buffer.ReadString());
    }

    public void Serialize(RegistryKeyArgument value, BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(value.Identifier);
    }
}
