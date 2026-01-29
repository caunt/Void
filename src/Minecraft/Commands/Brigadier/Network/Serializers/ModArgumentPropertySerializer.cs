using System;
using Void.Minecraft.Buffers;
using Void.Minecraft.Buffers.Extensions;
using Void.Minecraft.Commands.Brigadier.Network.Arguments;
using Void.Minecraft.Network;

namespace Void.Minecraft.Commands.Brigadier.Network.Serializers;

public class ModArgumentPropertySerializer : IArgumentPropertySerializer<ModArgumentProperty>
{
    public static IArgumentPropertySerializer<ModArgumentProperty> Instance { get; } = new ModArgumentPropertySerializer();

    public ModArgumentProperty Deserialize(BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        ArgumentIdentifier identifier;

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            var idx = buffer.ReadVarInt();
            identifier = ArgumentIdentifier.Id("crossstitch:identified_" + (idx < 0 ? "n" + (-idx) : idx), ArgumentIdentifier.MapSet(protocolVersion, idx));
        }
        else
        {
            identifier = ArgumentIdentifier.Id(buffer.ReadString());
        }

        return new ModArgumentProperty(identifier, new BufferMemory(buffer.ReadByteArray().ToArray()));
    }

    public void Serialize(ModArgumentProperty value, BufferSpan buffer, ProtocolVersion protocolVersion)
    {
        // This is special-cased by ArgumentPropertyRegistry
        throw new NotSupportedException();
    }
}
