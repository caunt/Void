using Void.Proxy.Network;
using Void.Proxy.Network.Protocol.Packets;

namespace Void.Proxy.Models.General;

public class Server(Link link)
{
    public Link Link { get; } = link;
    public string? Brand { get; protected set; }

    public void SetBrand(string brand)
    {
        Brand = brand;
    }

    public void EnableEncryption(byte[] secret, bool force = false)
    {
        if (!force)
            throw new NotSupportedException("Servers are always in offline mode, encryption doesn't work with it. Specify force to continue anyway.");

        Link.ServerChannel.EnableEncryption(secret);
        Proxy.Logger.Information($"Server {this} enabled encryption");
    }

    public void EnableCompression(int threshold)
    {
        Link.ServerChannel.EnableCompression(threshold);
        Proxy.Logger.Information($"Server {this} enabled compression");
    }

    public async Task SendPacketAsync(IMinecraftPacket packet)
    {
        await Link.SendPacketAsync(Direction.Serverbound, packet);
    }
}