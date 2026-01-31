using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.Serializers;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey;

public record ResourceOrTagKeyArgumentType(string Identifier) : RegistryKeyArgumentType(Identifier)
{
    public class Serializer : IArgumentSerializer
    {
        public static IArgumentSerializer Instance { get; } = new Serializer();

        public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
        {
            return new ResourceOrTagKeyArgumentType(buffer.ReadString());
        }

        public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
        {
            buffer.WriteString(value.As<ResourceOrTagKeyArgumentType>().Identifier);
        }
    }
}
