using System;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.ArgumentTypes;
using Void.Minecraft.Commands.Brigadier.Registry;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Serializers;

public class CrossStitchModArgumentSerializer : IArgumentSerializer
{
    public static IArgumentSerializer Instance { get; } = new CrossStitchModArgumentSerializer();

    public IArgumentType Deserialize(ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        ArgumentSerializerMapping identifier;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            var index = buffer.ReadVarInt();
            identifier = new ArgumentSerializerMapping("crossstitch:identified_" + (index < 0 ? "n" + (-index) : index), new() { [protocolVersion] = index });
        }
        else
        {
            identifier = new ArgumentSerializerMapping(buffer.ReadString());
        }

        return new CrossStitchModArgumentType(identifier, new BufferMemory(buffer.ReadByteArray().ToArray()));
    }

    public void Serialize(IArgumentType value, ref BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        throw new NotSupportedException();
    }
}
