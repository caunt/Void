namespace MinecraftProxy.Network.Protocol.Packets;

public interface IMinecraftPacket<in T> : IMinecraftPacket where T : IProtocolState
{
    public Task<bool> HandleAsync(T state);
}

public interface IMinecraftPacket
{
    public void Decode(MinecraftBuffer buffer);
    public void Encode(MinecraftBuffer buffer);
}