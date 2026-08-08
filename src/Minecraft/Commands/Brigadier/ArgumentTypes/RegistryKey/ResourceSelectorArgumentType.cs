using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.Serializers;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey;

/// <summary>Declares a resource-selector argument for a specific registry.</summary>
/// <param name="Identifier">The registry identifier.</param>
public record ResourceSelectorArgumentType(string Identifier) : RegistryKeyArgumentType(Identifier)
{
    /// <summary>
    /// Serializes <see cref="ResourceSelectorArgumentType"/> values by reading and writing their registry identifier string.
    /// </summary>
    public class Serializer : IArgumentSerializer
    {
        /// <summary>Gets the shared stateless serializer.</summary>
        public static IArgumentSerializer Instance { get; } = new Serializer();

        /// <inheritdoc/>
        public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
        {
            return new ResourceSelectorArgumentType(buffer.ReadString());
        }

        /// <inheritdoc/>
        public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
        {
            buffer.WriteString(value.As<ResourceSelectorArgumentType>().Identifier);
        }
    }
}
