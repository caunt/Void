using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Network.IO.Messages.Packets;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.API.Network.IO.Streams.Packet;

public interface IMinecraftPacketRegistryHolder
{
    public bool IsEmpty { get; }
    public ProtocolVersion? ProtocolVersion { get; set; }
    public IPlugin? ManagedBy { get; set; }
    public IMinecraftPacketRegistry Read { get; set; }
    public IMinecraftPacketRegistry Write { get; set; }

    public bool Contains<T>() where T : IMinecraftPacket;
    public void ReplacePackets(Operation operation, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings);
    public void AddPackets(Operation operation, IReadOnlyDictionary<MinecraftPacketMapping[], Type> mappings);
    public void Reset();
}