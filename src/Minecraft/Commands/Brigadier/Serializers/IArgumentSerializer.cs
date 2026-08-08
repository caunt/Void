using Void.Minecraft.Buffers;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers;

/// <summary>Converts Brigadier argument properties between objects and command-tree packet bytes.</summary>
public interface IArgumentSerializer
{
    /// <summary>Reads argument properties from a buffer.</summary>
    /// <param name="buffer">The source buffer.</param>
    /// <param name="protocolVersion">The source protocol version.</param>
    /// <returns>The decoded argument type or passthrough value.</returns>
    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion);
    /// <summary>Writes argument properties to a buffer.</summary>
    /// <param name="value">The argument type to encode.</param>
    /// <param name="buffer">The destination buffer.</param>
    /// <param name="protocolVersion">The target protocol version.</param>
    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion);
}
