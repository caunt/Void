using System;
using System.Collections.Generic;
using Void.Minecraft.Buffers;
using Void.Minecraft.Commands.Brigadier.Registry;
using Void.Minecraft.Commands.Brigadier.Serializers;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

/// <summary>Defines protocol argument metadata that can be serialized but cannot parse command input.</summary>
public interface IPassthroughArgumentValue : IArgumentType
{
    private const string NotSupportedMessage = $"This argument property is passthrough-only and is not supported. Consider implementing it as {nameof(IArgumentValue)}.";

    IEnumerable<string> IArgumentType.Examples => throw new NotSupportedException(NotSupportedMessage);
    IArgumentValue IArgumentType.Parse(StringReader reader) => throw new NotSupportedException(NotSupportedMessage);

    /// <summary>Gets the serializer responsible for this passthrough value.</summary>
    public IArgumentSerializer Serializer { get; }

    /// <summary>Writes this argument's protocol properties.</summary>
    /// <param name="buffer">The destination buffer.</param>
    /// <param name="protocolVersion">The target protocol version.</param>
    public virtual void Serialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        Serializer.Serialize(this, ref buffer, protocolVersion);
    }
}

/// <summary>Associates protocol-version serializer mappings with a passthrough argument value.</summary>
/// <param name="Mappings">The serializers selected by protocol version.</param>
/// <param name="Value">The protocol-only argument properties.</param>
public record PassthroughArgumentType(ArgumentSerializerMapping Mappings, IPassthroughArgumentValue Value) : IArgumentType
{
    /// <summary>Always throws because passthrough arguments do not expose parsing examples.</summary>
    /// <exception cref="NotSupportedException">Always thrown.</exception>
    public IEnumerable<string> Examples => throw new NotSupportedException();

    /// <summary>Always throws because passthrough arguments do not parse command input.</summary>
    /// <param name="reader">The unused reader.</param>
    /// <returns>This method does not return.</returns>
    /// <exception cref="NotSupportedException">Always thrown.</exception>
    public IArgumentValue Parse(StringReader reader) => throw new NotSupportedException();
}
