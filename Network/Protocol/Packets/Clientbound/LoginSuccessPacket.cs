using MinecraftProxy.Models;
using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Clientbound;

public struct LoginSuccessPacket : IMinecraftPacket<LoginState>
{
    public Guid Guid { get; set; }
    public string Username { get; set; }
    public List<Property> Properties { get; set; }

    public void Encode(ref MinecraftBuffer buffer)
    {
        buffer.WriteGuid(Guid);
        buffer.WriteString(Username);
        buffer.WritePropertyList(Properties);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer)
    {
        Guid = buffer.ReadGuid();
        Username = buffer.ReadString();
        Properties = buffer.ReadPropertyList();
    }
}

