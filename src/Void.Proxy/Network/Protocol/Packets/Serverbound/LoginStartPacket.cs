using Void.Proxy.Models.Minecraft.Encryption;
using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.States.Common;

namespace Void.Proxy.Network.Protocol.Packets.Serverbound;

public struct LoginStartPacket : IMinecraftPacket<LoginState>
{
    public string Username { get; set; }
    public Guid Guid { get; set; }
    public IdentifiedKey? IdentifiedKey { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteString(Username);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            if (protocolVersion < ProtocolVersion.MINECRAFT_1_19_3)
            {
                if (IdentifiedKey != null)
                {
                    buffer.WriteBoolean(true);
                    buffer.WriteIdentifiedKey(IdentifiedKey);
                }
                else
                {
                    buffer.WriteBoolean(false);
                }
            }

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
            {
                buffer.WriteGuid(Guid);
                return;
            }

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1)
            {
                if (IdentifiedKey != null && IdentifiedKey.Guid.HasValue)
                {
                    buffer.WriteBoolean(true);
                    buffer.WriteGuid(IdentifiedKey.Guid.Value);
                }
                else if (Guid != default)
                {
                    buffer.WriteBoolean(true);
                    buffer.WriteGuid(Guid);
                }
                else
                {
                    buffer.WriteBoolean(false);
                }
            }
        }
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        Username = buffer.ReadString();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_3)
            {
                IdentifiedKey = null;
            }
            else
            {
                if (buffer.ReadBoolean())
                {
                    IdentifiedKey = buffer.ReadIdentifiedKey(protocolVersion);
                }
                else
                {
                    IdentifiedKey = null;
                }
            }

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
            {
                Guid = buffer.ReadGuid();
                return;
            }

            if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19_1)
            {
                if (buffer.ReadBoolean())
                {
                    Guid = buffer.ReadGuid();
                }
            }
        }
        else
        {
            IdentifiedKey = null;
        }
    }
}

