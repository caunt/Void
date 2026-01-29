using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.Network.Serializers;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Arguments;

public static class RegistryKeyArgumentList
{
    public record ResourceOrTag(string Identifier) : RegistryKeyArgument(Identifier)
    {
        public class Serializer : IArgumentPropertySerializer<ResourceOrTag>
        {
            public static IArgumentPropertySerializer<ResourceOrTag> Instance { get; } = new Serializer();

            public ResourceOrTag? Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
            {
                return new ResourceOrTag(buffer.ReadString());
            }

            public void Serialize(ResourceOrTag value, BufferSpan buffer, ProtocolVersion protocolVersion)
            {
                buffer.WriteString(value.Identifier);
            }
        }
    }

    public record ResourceOrTagKey(string Identifier) : RegistryKeyArgument(Identifier)
    {
        public class Serializer : IArgumentPropertySerializer<ResourceOrTagKey>
        {
            public static IArgumentPropertySerializer<ResourceOrTagKey> Instance { get; } = new Serializer();

            public ResourceOrTagKey? Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
            {
                return new ResourceOrTagKey(buffer.ReadString());
            }

            public void Serialize(ResourceOrTagKey value, BufferSpan buffer, ProtocolVersion protocolVersion)
            {
                buffer.WriteString(value.Identifier);
            }
        }
    }

    public record ResourceSelector(string Identifier) : RegistryKeyArgument(Identifier)
    {
        public class Serializer : IArgumentPropertySerializer<ResourceSelector>
        {
            public static IArgumentPropertySerializer<ResourceSelector> Instance { get; } = new Serializer();

            public ResourceSelector? Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
            {
                return new ResourceSelector(buffer.ReadString());
            }

            public void Serialize(ResourceSelector value, BufferSpan buffer, ProtocolVersion protocolVersion)
            {
                buffer.WriteString(value.Identifier);
            }
        }
    }

    public record ResourceKey(string Identifier) : RegistryKeyArgument(Identifier)
    {
        public class Serializer : IArgumentPropertySerializer<ResourceKey>
        {
            public static IArgumentPropertySerializer<ResourceKey> Instance { get; } = new Serializer();

            public ResourceKey? Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
            {
                return new ResourceKey(buffer.ReadString());
            }

            public void Serialize(ResourceKey value, BufferSpan buffer, ProtocolVersion protocolVersion)
            {
                buffer.WriteString(value.Identifier);
            }
        }
    }
}
