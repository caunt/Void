using MinecraftProxy.Network.IO;
using MinecraftProxy.Network.Protocol.States.Common;

namespace MinecraftProxy.Network.Protocol.Packets.Serverbound;

public struct LoginStartPacket : IMinecraftPacket<LoginState>
{
    public string Username { get; set; }
    public Guid Guid { get; set; }

    public void Encode(ref MinecraftBuffer buffer)
    {
        buffer.WriteString(Username);
        buffer.WriteGuid(Guid);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(ref MinecraftBuffer buffer)
    {
        Username = buffer.ReadString();
        Guid = buffer.ReadGuid();
    }
}

