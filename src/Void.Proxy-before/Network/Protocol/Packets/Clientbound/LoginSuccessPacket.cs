using System.Text;
using Void.Proxy.Models.Minecraft.Profile;
using Void.Proxy.Network.IO;
using Void.Proxy.Network.Protocol.States.Common;

namespace Void.Proxy.Network.Protocol.Packets.Clientbound;

public struct LoginSuccessPacket : IMinecraftPacket<LoginState>
{
    public Guid Guid { get; set; }
    public string Username { get; set; }
    public List<Property>? Properties { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
            buffer.WriteGuid(Guid);
        else if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
            buffer.WriteGuidIntArray(Guid);
        else if (protocolVersion >= ProtocolVersion.MINECRAFT_1_7_6)
            buffer.WriteString(Guid.ToString());
        else
            buffer.WriteString(Guid.ToString().Replace("-", string.Empty));

        buffer.WriteString(Username);

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
        {
            if (Properties == null)
                buffer.WriteVarInt(0);
            else
                buffer.WritePropertyList(Properties);
        }
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
            Guid = buffer.ReadGuid();
        else if (protocolVersion >= ProtocolVersion.MINECRAFT_1_16)
            Guid = buffer.ReadGuidIntArray();
        else if (protocolVersion >= ProtocolVersion.MINECRAFT_1_7_6)
            Guid = Guid.Parse(buffer.ReadString(36));
        else
            Guid = Guid.Parse(buffer.ReadString(32));

        Username = buffer.ReadString();

        if (protocolVersion >= ProtocolVersion.MINECRAFT_1_19)
            Properties = buffer.ReadPropertyList();
    }

    public int MaxSize() => 64
        + 16
        + Encoding.UTF8.GetByteCount(Username)
        + Properties?.Sum(property => 0
            + Encoding.UTF8.GetByteCount(property.Name)
            + Encoding.UTF8.GetByteCount(property.Value)
            + 1
            + (property.Signature == null ? 0 : Encoding.UTF8.GetByteCount(property.Signature))) ?? 0;
}

