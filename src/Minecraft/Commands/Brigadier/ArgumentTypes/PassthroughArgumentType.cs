using System;
using System.Collections.Generic;
using Void.Minecraft.Buffers;
using Void.Minecraft.Commands.Brigadier.Registry;
using Void.Minecraft.Commands.Brigadier.Serializers;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.ArgumentTypes;

public interface IPassthroughArgumentValue : IArgumentType
{
    private const string NotSupportedMessage = $"This argument property is passthrough-only and is not supported. Consider implementing it as {nameof(IArgumentValue)}.";

    IEnumerable<string> IArgumentType.Examples => throw new NotSupportedException(NotSupportedMessage);
    IArgumentValue IArgumentType.Parse(StringReader reader) => throw new NotSupportedException(NotSupportedMessage);

    public IArgumentSerializer Serializer { get; }

    public virtual void Serialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        Serializer.Serialize(this, ref buffer, protocolVersion);
    }
}

public record PassthroughArgumentType(ArgumentSerializerMapping Mappings, IPassthroughArgumentValue Value) : IArgumentType
{
    public IEnumerable<string> Examples => throw new NotSupportedException();

    public IArgumentValue Parse(StringReader reader) => throw new NotSupportedException();
}
