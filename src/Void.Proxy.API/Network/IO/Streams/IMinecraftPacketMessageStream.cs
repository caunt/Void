using Void.Proxy.API.Network.IO.Messages;

namespace Void.Proxy.API.Network.IO.Streams;

public interface IMinecraftPacketMessageStream : IMinecraftStream
{
    public IMinecraftPacket ReadPacket();
    public ValueTask<IMinecraftPacket> ReadPacketAsync();
    public void WritePacket(IMinecraftPacket packet);
    public ValueTask WritePacketAsync(IMinecraftPacket packet);
}