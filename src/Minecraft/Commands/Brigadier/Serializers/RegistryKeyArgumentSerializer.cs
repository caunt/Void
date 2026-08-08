using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers;

/// <summary>Reads and writes registry-key argument identifiers as strings.</summary>
public class RegistryKeyArgumentSerializer : IArgumentSerializer
{
    /// <summary>Gets the shared stateless serializer.</summary>
    public static IArgumentSerializer Instance { get; } = new RegistryKeyArgumentSerializer();

    /// <inheritdoc/>
    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        return new RegistryKeyArgumentType(Identifier: buffer.ReadString());
    }

    /// <inheritdoc/>
    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(value.As<RegistryKeyArgumentType>().Identifier);
    }
}
