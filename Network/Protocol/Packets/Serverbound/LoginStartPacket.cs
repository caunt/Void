using MinecraftProxy.Network.Protocol.States;

namespace MinecraftProxy.Network.Protocol.Packets.Serverbound;

public class LoginStartPacket : IMinecraftPacket<LoginState>
{
    public string Username { get; set; }
    public Guid Guid { get; set; }


    public void Encode(MinecraftBuffer buffer)
    {
        buffer.WriteString(Username);
        buffer.WriteGuid(Guid);
    }

    public async Task<bool> HandleAsync(LoginState state) => await state.HandleAsync(this);

    public void Decode(MinecraftBuffer buffer)
    {
        Username = buffer.ReadString();
        Guid = buffer.ReadGuid();
    }
}

