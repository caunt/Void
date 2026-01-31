using System.Collections.Frozen;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Registry;
using Void.Minecraft.Commands.Brigadier.Serializers;
using Void.Minecraft.Network;

namespace Void.Proxy.Plugins.ModsSupport.CrossStitch;

public class CrossStitchModArgumentSerializer : IArgumentSerializer
{
    private static readonly FrozenDictionary<ProtocolVersion, int> _headerSizes = ProtocolVersion.Range().ToFrozenDictionary(version => version, version =>
    {
        var mapping = CrossStitchService.ModArgumentMapping;
        var buffer = new BufferSpan(stackalloc byte[mapping.Identifier.Length * 2]);

        ArgumentSerializerRegistry.WriteParserIdentifier(ref buffer, mapping, version);
        return buffer.Position;
    });

    public static IArgumentSerializer Instance { get; } = new CrossStitchModArgumentSerializer();

    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        ArgumentSerializerMapping identifier;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            var parserId = buffer.ReadVarInt();
            identifier = new ArgumentSerializerMapping("crossstitch:identified_" + (parserId < 0 ? "n" + (-parserId) : parserId), protocolVersion, parserId);
        }
        else
        {
            identifier = new ArgumentSerializerMapping(buffer.ReadString());
        }

        return new CrossStitchModArgumentType(identifier, new BufferMemory(buffer.ReadByteArray().ToArray()));
    }

    public void Serialize(IArgumentType argumentType, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        var modArgumentType = argumentType.As<CrossStitchModArgumentType>();
        var unwrappedMapping = modArgumentType.Mapping;

        // ArgumentSerializerRegistry already wrote the identifier, we need to go back and overwrite it with unwrapped one
        var modArgumentHeaderSize = _headerSizes[protocolVersion];
        buffer.Seek(-modArgumentHeaderSize);

        ArgumentSerializerRegistry.WriteParserIdentifier(ref buffer, unwrappedMapping, protocolVersion);

        var span = modArgumentType.Data.Span;
        buffer.Write(span.ReadToEnd());
    }
}
