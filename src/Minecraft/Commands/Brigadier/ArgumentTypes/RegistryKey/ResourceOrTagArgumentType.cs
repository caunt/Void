using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.Serializers;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey;

/// <summary>Declares a resource-or-tag argument for a specific registry.</summary>
/// <param name="Identifier">The registry identifier.</param>
public record ResourceOrTagArgumentType(string Identifier) : RegistryKeyArgumentType(Identifier)
{
    /// <summary>Reads and writes the registry identifier for resource-or-tag arguments.</summary>
    public class Serializer : IArgumentSerializer
    {
        /// <summary>Gets the shared stateless serializer.</summary>
        public static IArgumentSerializer Instance { get; } = new Serializer();

        /// <inheritdoc/>
        public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
        {
            return new ResourceOrTagArgumentType(buffer.ReadString());
        }

        /// <inheritdoc/>
        public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
        {
            buffer.WriteString(value.As<ResourceOrTagArgumentType>().Identifier);
        }
    }
}
